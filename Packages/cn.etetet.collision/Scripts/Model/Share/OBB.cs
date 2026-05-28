using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace ET
{
    /// <summary>
    /// 有向包围盒（Oriented Bounding Box），即带方向的立方体。
    /// 不可变值类型，以中心点 + 半长 + 旋转存储。
    /// </summary>
    public readonly struct OBB
    {
        /// <summary>中心点。</summary>
        public readonly float3 Center;

        /// <summary>未旋转时各轴半长，恒为非负。</summary>
        public readonly float3 HalfExtents;

        /// <summary>朝向旋转。</summary>
        public readonly quaternion Rotation;

        /// <summary>以中心点、半长、旋转构造。</summary>
        public OBB(float3 center, float3 halfExtents, quaternion rotation)
        {
            this.Center = center;
            this.HalfExtents = math.abs(halfExtents);
            this.Rotation = rotation;
        }

        /// <summary>世界空间下的局部 X 轴（已归一化）。</summary>
        public float3 AxisX
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => math.mul(this.Rotation, math.right());
        }

        /// <summary>世界空间下的局部 Y 轴（已归一化）。</summary>
        public float3 AxisY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => math.mul(this.Rotation, math.up());
        }

        /// <summary>世界空间下的局部 Z 轴（已归一化）。</summary>
        public float3 AxisZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => math.mul(this.Rotation, math.forward());
        }

        /// <summary>由一个 <see cref="AABB"/> 构造（旋转为单位四元数）。</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OBB FromAABB(in AABB aabb)
        {
            return new OBB(aabb.Center, aabb.HalfExtents, quaternion.identity);
        }
    }
}
