using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace ET
{
    /// <summary>
    /// 轴对齐包围盒（Axis-Aligned Bounding Box）。
    /// 不可变值类型，以中心点 + 半长（half extents）存储，便于与 <see cref="OBB"/> 统一计算。
    /// </summary>
    public readonly struct AABB
    {
        /// <summary>中心点。</summary>
        public readonly float3 Center;

        /// <summary>各轴半长，恒为非负。</summary>
        public readonly float3 HalfExtents;

        /// <summary>以中心点和半长构造。</summary>
        public AABB(float3 center, float3 halfExtents)
        {
            this.Center = center;
            this.HalfExtents = math.abs(halfExtents);
        }

        /// <summary>最小角点。</summary>
        public float3 Min
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.Center - this.HalfExtents;
        }

        /// <summary>最大角点。</summary>
        public float3 Max
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.Center + this.HalfExtents;
        }

        /// <summary>完整尺寸（= 2 * 半长）。</summary>
        public float3 Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.HalfExtents * 2f;
        }

        /// <summary>由最小/最大角点构造。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AABB FromMinMax(float3 min, float3 max)
        {
            return new AABB((min + max) * 0.5f, (max - min) * 0.5f);
        }

        /// <summary>由中心点和完整尺寸构造。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AABB FromCenterSize(float3 center, float3 size)
        {
            return new AABB(center, size * 0.5f);
        }
    }
}
