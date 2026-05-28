using System.Runtime.CompilerServices;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace ET
{
    /// <summary>
    /// 高性能 3D 碰撞检测对外接口。全部为无分配（allocation-free）的静态值类型计算。
    ///
    /// 支持：AABB-AABB、Sphere-Sphere、OBB-OBB（带方向立方体之间）、OBB-Sphere（带方向立方体与球体），
    /// 以及 AABB-Sphere、AABB-OBB 的派生组合，并提供 <see cref="Collider3D"/> 统一多态派发。
    /// </summary>
    public static class Collision3DHelper
    {
        // 抵消两条平行边叉乘接近零向量时的浮点误差。
        private const float Epsilon = 1e-6f;

        // ---------------------------------------------------------------------
        // 同形状检测
        // ---------------------------------------------------------------------

        /// <summary>AABB 与 AABB 是否相交（接触即算相交）。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AABBIntersect(in AABB a, in AABB b)
        {
            // 各轴投影区间均重叠才相交。
            return all(abs(a.Center - b.Center) <= (a.HalfExtents + b.HalfExtents));
        }

        /// <summary>球体与球体是否相交。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SphereIntersect(in Sphere a, in Sphere b)
        {
            float3 d = a.Center - b.Center;
            float r = a.Radius + b.Radius;
            return lengthsq(d) <= r * r;
        }

        /// <summary>有向立方体与有向立方体是否相交（分离轴定理 SAT，15 条候选轴）。</summary>
        public static bool OBBIntersect(in OBB a, in OBB b)
        {
            float3x3 ra3 = new float3x3(a.Rotation);
            float3x3 rb3 = new float3x3(b.Rotation);

            float3 au0 = ra3.c0, au1 = ra3.c1, au2 = ra3.c2;
            float3 bu0 = rb3.c0, bu1 = rb3.c1, bu2 = rb3.c2;

            // R[i][j] = dot(A_i, B_j)：把 b 的朝向表达到 a 的坐标系。
            float r00 = dot(au0, bu0), r01 = dot(au0, bu1), r02 = dot(au0, bu2);
            float r10 = dot(au1, bu0), r11 = dot(au1, bu1), r12 = dot(au1, bu2);
            float r20 = dot(au2, bu0), r21 = dot(au2, bu1), r22 = dot(au2, bu2);

            // 平移向量带入 a 的坐标系。
            float3 tw = b.Center - a.Center;
            float t0 = dot(tw, au0), t1 = dot(tw, au1), t2 = dot(tw, au2);

            // 预加 Epsilon 抵消近平行叉乘误差。
            float ar00 = abs(r00) + Epsilon, ar01 = abs(r01) + Epsilon, ar02 = abs(r02) + Epsilon;
            float ar10 = abs(r10) + Epsilon, ar11 = abs(r11) + Epsilon, ar12 = abs(r12) + Epsilon;
            float ar20 = abs(r20) + Epsilon, ar21 = abs(r21) + Epsilon, ar22 = abs(r22) + Epsilon;

            float ea0 = a.HalfExtents.x, ea1 = a.HalfExtents.y, ea2 = a.HalfExtents.z;
            float eb0 = b.HalfExtents.x, eb1 = b.HalfExtents.y, eb2 = b.HalfExtents.z;

            float lhs, rhs;

            // L = A0, A1, A2
            rhs = ea0 + eb0 * ar00 + eb1 * ar01 + eb2 * ar02;
            if (abs(t0) > rhs) return false;
            rhs = ea1 + eb0 * ar10 + eb1 * ar11 + eb2 * ar12;
            if (abs(t1) > rhs) return false;
            rhs = ea2 + eb0 * ar20 + eb1 * ar21 + eb2 * ar22;
            if (abs(t2) > rhs) return false;

            // L = B0, B1, B2
            rhs = ea0 * ar00 + ea1 * ar10 + ea2 * ar20 + eb0;
            if (abs(t0 * r00 + t1 * r10 + t2 * r20) > rhs) return false;
            rhs = ea0 * ar01 + ea1 * ar11 + ea2 * ar21 + eb1;
            if (abs(t0 * r01 + t1 * r11 + t2 * r21) > rhs) return false;
            rhs = ea0 * ar02 + ea1 * ar12 + ea2 * ar22 + eb2;
            if (abs(t0 * r02 + t1 * r12 + t2 * r22) > rhs) return false;

            // L = A0 x B0
            lhs = abs(t2 * r10 - t1 * r20);
            rhs = ea1 * ar20 + ea2 * ar10 + eb1 * ar02 + eb2 * ar01;
            if (lhs > rhs) return false;
            // L = A0 x B1
            lhs = abs(t2 * r11 - t1 * r21);
            rhs = ea1 * ar21 + ea2 * ar11 + eb0 * ar02 + eb2 * ar00;
            if (lhs > rhs) return false;
            // L = A0 x B2
            lhs = abs(t2 * r12 - t1 * r22);
            rhs = ea1 * ar22 + ea2 * ar12 + eb0 * ar01 + eb1 * ar00;
            if (lhs > rhs) return false;
            // L = A1 x B0
            lhs = abs(t0 * r20 - t2 * r00);
            rhs = ea0 * ar20 + ea2 * ar00 + eb1 * ar12 + eb2 * ar11;
            if (lhs > rhs) return false;
            // L = A1 x B1
            lhs = abs(t0 * r21 - t2 * r01);
            rhs = ea0 * ar21 + ea2 * ar01 + eb0 * ar12 + eb2 * ar10;
            if (lhs > rhs) return false;
            // L = A1 x B2
            lhs = abs(t0 * r22 - t2 * r02);
            rhs = ea0 * ar22 + ea2 * ar02 + eb0 * ar11 + eb1 * ar10;
            if (lhs > rhs) return false;
            // L = A2 x B0
            lhs = abs(t1 * r00 - t0 * r10);
            rhs = ea0 * ar10 + ea1 * ar00 + eb1 * ar22 + eb2 * ar21;
            if (lhs > rhs) return false;
            // L = A2 x B1
            lhs = abs(t1 * r01 - t0 * r11);
            rhs = ea0 * ar11 + ea1 * ar01 + eb0 * ar22 + eb2 * ar20;
            if (lhs > rhs) return false;
            // L = A2 x B2
            lhs = abs(t1 * r02 - t0 * r12);
            rhs = ea0 * ar12 + ea1 * ar02 + eb0 * ar21 + eb1 * ar20;
            if (lhs > rhs) return false;

            // 找不到分离轴，必相交。
            return true;
        }

        // ---------------------------------------------------------------------
        // 异形状检测
        // ---------------------------------------------------------------------

        /// <summary>有向立方体与球体是否相交（取立方体上离球心最近点，比较距离与半径）。</summary>
        public static bool OBBSphereIntersect(in OBB obb, in Sphere sphere)
        {
            float3x3 r = new float3x3(obb.Rotation);
            float3 d = sphere.Center - obb.Center;

            // 球心带入立方体局部坐标系。
            float3 local = new float3(dot(d, r.c0), dot(d, r.c1), dot(d, r.c2));
            // 夹取到盒内得到最近点；旋转正交，局部距离即世界距离。
            float3 clamped = clamp(local, -obb.HalfExtents, obb.HalfExtents);
            float3 diff = local - clamped;
            return lengthsq(diff) <= sphere.Radius * sphere.Radius;
        }

        /// <summary>AABB 与球体是否相交（取盒上离球心最近点，比较距离与半径）。</summary>
        public static bool AABBSphereIntersect(in AABB aabb, in Sphere sphere)
        {
            float3 q = clamp(sphere.Center, aabb.Min, aabb.Max);
            float3 diff = sphere.Center - q;
            return lengthsq(diff) <= sphere.Radius * sphere.Radius;
        }

        /// <summary>AABB 与有向立方体是否相交（AABB 视为单位旋转的 OBB，复用 SAT）。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AABBOBBIntersect(in AABB aabb, in OBB obb)
        {
            OBB a = OBB.FromAABB(aabb);
            return OBBIntersect(a, obb);
        }

        // ---------------------------------------------------------------------
        // 点包含
        // ---------------------------------------------------------------------

        /// <summary>点是否在 AABB 内（含边界）。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsPoint(in AABB aabb, float3 point)
        {
            return all(abs(point - aabb.Center) <= aabb.HalfExtents);
        }

        /// <summary>点是否在有向立方体内（含边界）。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsPoint(in OBB obb, float3 point)
        {
            float3x3 r = new float3x3(obb.Rotation);
            float3 d = point - obb.Center;
            float3 local = new float3(dot(d, r.c0), dot(d, r.c1), dot(d, r.c2));
            return all(abs(local) <= obb.HalfExtents);
        }

        /// <summary>点是否在球体内（含边界）。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsPoint(in Sphere sphere, float3 point)
        {
            return lengthsq(point - sphere.Center) <= sphere.Radius * sphere.Radius;
        }

        // ---------------------------------------------------------------------
        // 统一多态派发
        // ---------------------------------------------------------------------

        /// <summary>任意两个 <see cref="Collider3D"/> 是否相交，按形状类型派发到对应算法。</summary>
        public static bool Intersect(in Collider3D a, in Collider3D b)
        {
            switch (a.ShapeType)
            {
                case CollisionShapeType.AABB:
                    switch (b.ShapeType)
                    {
                        case CollisionShapeType.AABB: return AABBIntersect(a.AsAABB(), b.AsAABB());
                        case CollisionShapeType.OBB: return AABBOBBIntersect(a.AsAABB(), b.AsOBB());
                        case CollisionShapeType.Sphere: return AABBSphereIntersect(a.AsAABB(), b.AsSphere());
                    }
                    break;
                case CollisionShapeType.OBB:
                    switch (b.ShapeType)
                    {
                        case CollisionShapeType.AABB: return AABBOBBIntersect(b.AsAABB(), a.AsOBB());
                        case CollisionShapeType.OBB: return OBBIntersect(a.AsOBB(), b.AsOBB());
                        case CollisionShapeType.Sphere: return OBBSphereIntersect(a.AsOBB(), b.AsSphere());
                    }
                    break;
                case CollisionShapeType.Sphere:
                    switch (b.ShapeType)
                    {
                        case CollisionShapeType.AABB: return AABBSphereIntersect(b.AsAABB(), a.AsSphere());
                        case CollisionShapeType.OBB: return OBBSphereIntersect(b.AsOBB(), a.AsSphere());
                        case CollisionShapeType.Sphere: return SphereIntersect(a.AsSphere(), b.AsSphere());
                    }
                    break;
            }
            return false;
        }
    }
}
