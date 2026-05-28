using Unity.Mathematics;

namespace ET
{
    /// <summary>
    /// 球体。不可变值类型，以中心点 + 半径存储。
    /// </summary>
    public readonly struct Sphere
    {
        /// <summary>球心。</summary>
        public readonly float3 Center;

        /// <summary>半径，恒为非负。</summary>
        public readonly float Radius;

        /// <summary>以球心和半径构造。</summary>
        public Sphere(float3 center, float radius)
        {
            this.Center = center;
            this.Radius = math.abs(radius);
        }
    }
}
