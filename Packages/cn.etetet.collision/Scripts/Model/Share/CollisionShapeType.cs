namespace ET
{
    /// <summary>
    /// 3D 碰撞体形状类型。用于 <see cref="Collider3D"/> 统一派发。
    /// </summary>
    public enum CollisionShapeType : byte
    {
        /// <summary>轴对齐包围盒</summary>
        AABB = 0,

        /// <summary>带方向的立方体（有向包围盒）</summary>
        OBB = 1,

        /// <summary>球体</summary>
        Sphere = 2,
    }
}
