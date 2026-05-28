using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace ET
{
    /// <summary>
    /// 统一碰撞体。把 <see cref="AABB"/> / <see cref="OBB"/> / <see cref="Sphere"/> 收敛到一个值类型，
    /// 便于在异构碰撞体集合上做运行时多态检测（见 <see cref="Collision3DHelper.Intersect(in Collider3D, in Collider3D)"/>）。
    /// </summary>
    public readonly struct Collider3D
    {
        /// <summary>形状类型，决定哪些字段有效。</summary>
        public readonly CollisionShapeType ShapeType;

        /// <summary>中心点，所有形状均有效。</summary>
        public readonly float3 Center;

        /// <summary>各轴半长，<see cref="CollisionShapeType.AABB"/> / <see cref="CollisionShapeType.OBB"/> 有效。</summary>
        public readonly float3 HalfExtents;

        /// <summary>朝向旋转，仅 <see cref="CollisionShapeType.OBB"/> 有效。</summary>
        public readonly quaternion Rotation;

        /// <summary>半径，仅 <see cref="CollisionShapeType.Sphere"/> 有效。</summary>
        public readonly float Radius;

        private Collider3D(CollisionShapeType shapeType, float3 center, float3 halfExtents, quaternion rotation, float radius)
        {
            this.ShapeType = shapeType;
            this.Center = center;
            this.HalfExtents = halfExtents;
            this.Rotation = rotation;
            this.Radius = radius;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Collider3D Create(in AABB aabb)
        {
            return new Collider3D(CollisionShapeType.AABB, aabb.Center, aabb.HalfExtents, quaternion.identity, 0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Collider3D Create(in OBB obb)
        {
            return new Collider3D(CollisionShapeType.OBB, obb.Center, obb.HalfExtents, obb.Rotation, 0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Collider3D Create(in Sphere sphere)
        {
            return new Collider3D(CollisionShapeType.Sphere, sphere.Center, default, quaternion.identity, sphere.Radius);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AABB AsAABB()
        {
            return new AABB(this.Center, this.HalfExtents);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OBB AsOBB()
        {
            return new OBB(this.Center, this.HalfExtents, this.Rotation);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Sphere AsSphere()
        {
            return new Sphere(this.Center, this.Radius);
        }
    }
}
