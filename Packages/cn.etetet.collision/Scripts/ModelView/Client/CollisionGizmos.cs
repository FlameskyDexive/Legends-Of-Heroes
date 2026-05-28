#if UNITY_EDITOR
using Unity.Mathematics;
using UnityEngine;

namespace ET.Client
{
    /// <summary>
    /// 碰撞体 Gizmos 绘制器，供编辑器 Scene 视图可视化调试使用。
    /// 仅在 UNITY_EDITOR 下编译，需在 MonoBehaviour 的 OnDrawGizmos 内调用。
    /// </summary>
    public static class CollisionGizmos
    {
        /// <summary>绘制 AABB 线框。</summary>
        public static void DrawAABB(in AABB aabb, Color color)
        {
            Matrix4x4 old = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = color;
            Vector3 center = aabb.Center;
            Vector3 size = aabb.Size;
            Gizmos.DrawWireCube(center, size);
            Gizmos.matrix = old;
        }

        /// <summary>绘制有向立方体（OBB）线框，通过 Gizmos 矩阵施加旋转。</summary>
        public static void DrawOBB(in OBB obb, Color color)
        {
            Matrix4x4 old = Gizmos.matrix;
            Vector3 center = obb.Center;
            Quaternion rotation = obb.Rotation;
            Gizmos.color = color;
            Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
            Vector3 size = obb.HalfExtents * 2f;
            Gizmos.DrawWireCube(Vector3.zero, size);
            Gizmos.matrix = old;
        }

        /// <summary>绘制球体线框。</summary>
        public static void DrawSphere(in Sphere sphere, Color color)
        {
            Matrix4x4 old = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = color;
            Vector3 center = sphere.Center;
            Gizmos.DrawWireSphere(center, sphere.Radius);
            Gizmos.matrix = old;
        }

        /// <summary>按形状类型派发绘制统一碰撞体。</summary>
        public static void DrawCollider(in Collider3D collider, Color color)
        {
            switch (collider.ShapeType)
            {
                case CollisionShapeType.AABB:
                    DrawAABB(collider.AsAABB(), color);
                    break;
                case CollisionShapeType.OBB:
                    DrawOBB(collider.AsOBB(), color);
                    break;
                case CollisionShapeType.Sphere:
                    DrawSphere(collider.AsSphere(), color);
                    break;
            }
        }
    }
}
#endif
