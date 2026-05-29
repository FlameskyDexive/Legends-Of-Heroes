using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace ET
{
    /// <summary>
    /// 3D 碰撞世界：对大量碰撞体做高性能批量相交检测。
    ///
    /// 「广相 + 窄相」两级流水线：
    /// - 广相用稠密均匀网格（dense uniform grid）：每个碰撞体按其世界 AABB 中心落入单格，
    ///   只与 3x3x3 邻域比较，三轴同时剪枝，接近 O(N)。网格用线性下标寻址（无哈希）。
    /// - 广相的 AABB 重叠判定用标量 float 比较（避开非 Burst 环境下 Unity.Mathematics 向量运算的开销）。
    /// - 窄相用 <see cref="Collision3DHelper"/> 精确判定，只对极少数候选对调用。
    ///
    /// 网格边长自动取所有碰撞体最大的 AABB 边长（保证相交两体中心必在 3x3x3 邻域内），
    /// 并对网格总数封顶（超限时增大边长），避免极端尺寸/范围下内存爆炸。
    /// 复用内部缓冲，单帧检测零堆分配（结果 List 由调用方复用）。
    /// 典型用法：每帧 <see cref="Clear"/> → 多次 <see cref="Add"/> → <see cref="DetectPairs"/>。
    /// 普通数据结构（非 ET Object），按本项目约定加 [EnableClass] 以通过 Model 程序集分析器（ET0032）。
    /// </summary>
    [EnableClass]
    public sealed class CollisionWorld3D
    {
        private const int MaxCells = 1 << 21; // 网格总格数上限（约 200 万），约 8MB 头表。

        private Collider3D[] colliders;
        private float[] minX, minY, minZ, maxX, maxY, maxZ;
        private int[] next, cellX, cellY, cellZ;
        private int[] head = new int[1024]; // 稠密网格头表，按需扩容。
        private int count;
        private float maxDim;

        public CollisionWorld3D(int capacity = 256)
        {
            capacity = capacity < 16 ? 16 : capacity;
            this.colliders = new Collider3D[capacity];
            this.minX = new float[capacity];
            this.minY = new float[capacity];
            this.minZ = new float[capacity];
            this.maxX = new float[capacity];
            this.maxY = new float[capacity];
            this.maxZ = new float[capacity];
            this.next = new int[capacity];
            this.cellX = new int[capacity];
            this.cellY = new int[capacity];
            this.cellZ = new int[capacity];
        }

        public int Count => this.count;

        public void Clear()
        {
            this.count = 0;
            this.maxDim = 0f;
        }

        public int Add(in Collider3D collider)
        {
            if (this.count >= this.colliders.Length)
            {
                int n = this.colliders.Length * 2;
                Array.Resize(ref this.colliders, n);
                Array.Resize(ref this.minX, n);
                Array.Resize(ref this.minY, n);
                Array.Resize(ref this.minZ, n);
                Array.Resize(ref this.maxX, n);
                Array.Resize(ref this.maxY, n);
                Array.Resize(ref this.maxZ, n);
                Array.Resize(ref this.next, n);
                Array.Resize(ref this.cellX, n);
                Array.Resize(ref this.cellY, n);
                Array.Resize(ref this.cellZ, n);
            }

            int i = this.count;
            this.colliders[i] = collider;

            AABB b = Collision3DHelper.ComputeAABB(collider);
            float3 mn = b.Min;
            float3 mx = b.Max;
            this.minX[i] = mn.x; this.minY[i] = mn.y; this.minZ[i] = mn.z;
            this.maxX[i] = mx.x; this.maxY[i] = mx.y; this.maxZ[i] = mx.z;

            float dx = mx.x - mn.x, dy = mx.y - mn.y, dz = mx.z - mn.z;
            float d = dx > dy ? dx : dy;
            if (dz > d) d = dz;
            if (d > this.maxDim) this.maxDim = d;

            this.count++;
            return i;
        }

        public AABB GetBounds(int index)
        {
            return new AABB(
                new float3((this.minX[index] + this.maxX[index]) * 0.5f, (this.minY[index] + this.maxY[index]) * 0.5f, (this.minZ[index] + this.maxZ[index]) * 0.5f),
                new float3((this.maxX[index] - this.minX[index]) * 0.5f, (this.maxY[index] - this.minY[index]) * 0.5f, (this.maxZ[index] - this.minZ[index]) * 0.5f));
        }

        /// <summary>
        /// 检测所有相交对，结果追加到 <paramref name="results"/>（调用前会先 Clear）。每对满足 A &lt; B。
        /// </summary>
        public void DetectPairs(List<CollisionPair> results)
        {
            results.Clear();
            int n = this.count;
            if (n < 2)
            {
                return;
            }

            // 世界 AABB
            float wMinX = this.minX[0], wMinY = this.minY[0], wMinZ = this.minZ[0];
            float wMaxX = this.maxX[0], wMaxY = this.maxY[0], wMaxZ = this.maxZ[0];
            for (int i = 1; i < n; i++)
            {
                if (this.minX[i] < wMinX) wMinX = this.minX[i];
                if (this.minY[i] < wMinY) wMinY = this.minY[i];
                if (this.minZ[i] < wMinZ) wMinZ = this.minZ[i];
                if (this.maxX[i] > wMaxX) wMaxX = this.maxX[i];
                if (this.maxY[i] > wMaxY) wMaxY = this.maxY[i];
                if (this.maxZ[i] > wMaxZ) wMaxZ = this.maxZ[i];
            }

            float cell = this.maxDim > 1e-5f ? this.maxDim : 1f;
            float spanX = wMaxX - wMinX, spanY = wMaxY - wMinY, spanZ = wMaxZ - wMinZ;

            // 计算网格维度（含两侧各 1 格余量），并对总格数封顶。
            int nx, ny, nz;
            while (true)
            {
                float inv0 = 1f / cell;
                nx = (int)(spanX * inv0) + 3;
                ny = (int)(spanY * inv0) + 3;
                nz = (int)(spanZ * inv0) + 3;
                if ((long)nx * ny * nz <= MaxCells)
                {
                    break;
                }
                cell *= 1.2599f; // ~cbrt(2)，增大边长以压缩格数
            }

            float inv = 1f / cell;
            float originX = wMinX - cell, originY = wMinY - cell, originZ = wMinZ - cell;
            int cellCount = nx * ny * nz;

            if (this.head.Length < cellCount)
            {
                this.head = new int[cellCount];
            }
            Array.Fill(this.head, -1, 0, cellCount);

            int nxy = nx * ny;

            // 建网格：每个碰撞体按中心落入单格，头插法链入。
            for (int i = 0; i < n; i++)
            {
                int cx = (int)(((this.minX[i] + this.maxX[i]) * 0.5f - originX) * inv);
                int cy = (int)(((this.minY[i] + this.maxY[i]) * 0.5f - originY) * inv);
                int cz = (int)(((this.minZ[i] + this.maxZ[i]) * 0.5f - originZ) * inv);
                if (cx < 0) cx = 0; else if (cx >= nx) cx = nx - 1;
                if (cy < 0) cy = 0; else if (cy >= ny) cy = ny - 1;
                if (cz < 0) cz = 0; else if (cz >= nz) cz = nz - 1;
                this.cellX[i] = cx;
                this.cellY[i] = cy;
                this.cellZ[i] = cz;
                int ci = cz * nxy + cy * nx + cx;
                this.next[i] = this.head[ci];
                this.head[ci] = i;
            }

            // 查询：每个碰撞体只与 3x3x3 邻域比较；j > i 保证每对只出一次。
            for (int i = 0; i < n; i++)
            {
                float iMnX = this.minX[i], iMnY = this.minY[i], iMnZ = this.minZ[i];
                float iMxX = this.maxX[i], iMxY = this.maxY[i], iMxZ = this.maxZ[i];
                int cx = this.cellX[i], cy = this.cellY[i], cz = this.cellZ[i];

                int zLo = cz > 0 ? cz - 1 : 0, zHi = cz < nz - 1 ? cz + 1 : nz - 1;
                int yLo = cy > 0 ? cy - 1 : 0, yHi = cy < ny - 1 ? cy + 1 : ny - 1;
                int xLo = cx > 0 ? cx - 1 : 0, xHi = cx < nx - 1 ? cx + 1 : nx - 1;

                for (int z = zLo; z <= zHi; z++)
                {
                    int zb = z * nxy;
                    for (int y = yLo; y <= yHi; y++)
                    {
                        int yb = zb + y * nx;
                        for (int x = xLo; x <= xHi; x++)
                        {
                            for (int j = this.head[yb + x]; j != -1; j = this.next[j])
                            {
                                if (j <= i)
                                {
                                    continue;
                                }
                                if (iMnX > this.maxX[j] || iMxX < this.minX[j]) continue;
                                if (iMnY > this.maxY[j] || iMxY < this.minY[j]) continue;
                                if (iMnZ > this.maxZ[j] || iMxZ < this.minZ[j]) continue;

                                if (Collision3DHelper.Intersect(this.colliders[i], this.colliders[j]))
                                {
                                    results.Add(new CollisionPair(i, j));
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
