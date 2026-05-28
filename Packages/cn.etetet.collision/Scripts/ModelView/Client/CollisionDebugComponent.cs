#if UNITY_EDITOR
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEditor;

namespace ET.Client
{
    /// <summary>
    /// 挂到场景 GameObject 上的碰撞可视化调试器（仅 UNITY_EDITOR 下编译）。在 Inspector 中配置若干碰撞体，
    /// Scene 视图实时绘制并两两检测：发生相交的碰撞体以高亮色显示。
    /// 形状中心、旋转相对所在 Transform，可直接拖拽物体观察碰撞结果。
    ///
    /// 本类是纯 Unity 编辑器调试用的 MonoBehaviour（非 ET Object），按本项目约定加 [EnableClass]
    /// 以通过 ModelView 程序集的普通类声明分析器（ET0032）。
    /// </summary>
    [EnableClass]
    public class CollisionDebugComponent : MonoBehaviour
    {
        /// <summary>单个可配置碰撞体。</summary>
        [System.Serializable]
        [EnableClass]
        public class DebugShape
        {
            public CollisionShapeType Type = CollisionShapeType.AABB;

            [Tooltip("相对所在 Transform 的局部中心点")]
            public Vector3 Center = Vector3.zero;

            [Tooltip("立方体完整尺寸（AABB / OBB 有效）")]
            public Vector3 Size = Vector3.one;

            [Tooltip("欧拉角（仅 OBB 有效）")]
            public Vector3 EulerAngles = Vector3.zero;

            [Tooltip("半径（仅 Sphere 有效）")]
            public float Radius = 0.5f;
        }

        public List<DebugShape> Shapes = new List<DebugShape>();

        [Tooltip("未相交时的颜色")]
        public Color NormalColor = Color.green;

        [Tooltip("相交时的高亮颜色")]
        public Color HitColor = Color.red;

        [Tooltip("是否在碰撞体上方显示类型/命中文字")]
        public bool ShowLabels = true;

        private void OnDrawGizmos()
        {
            if (this.Shapes == null || this.Shapes.Count == 0)
            {
                return;
            }

            int n = this.Shapes.Count;
            Collider3D[] colliders = new Collider3D[n];
            bool[] hit = new bool[n];

            for (int i = 0; i < n; i++)
            {
                colliders[i] = this.BuildCollider(this.Shapes[i]);
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (Collision3DHelper.Intersect(colliders[i], colliders[j]))
                    {
                        hit[i] = true;
                        hit[j] = true;
                    }
                }
            }

            for (int i = 0; i < n; i++)
            {
                Color color = hit[i] ? this.HitColor : this.NormalColor;
                CollisionGizmos.DrawCollider(colliders[i], color);

                if (this.ShowLabels)
                {
                    Vector3 labelPos = colliders[i].Center;
                    Handles.color = color;
                    Handles.Label(labelPos, hit[i] ? $"{this.Shapes[i].Type} (HIT)" : this.Shapes[i].Type.ToString());
                }
            }
        }

        // 把 Inspector 配置结合所在 Transform 转换为世界空间碰撞体。
        private Collider3D BuildCollider(DebugShape shape)
        {
            float3 worldCenter = this.transform.TransformPoint(shape.Center);
            switch (shape.Type)
            {
                case CollisionShapeType.OBB:
                {
                    quaternion rotation = this.transform.rotation * Quaternion.Euler(shape.EulerAngles);
                    float3 halfExtents = (float3)shape.Size * 0.5f;
                    return Collider3D.Create(new OBB(worldCenter, halfExtents, rotation));
                }
                case CollisionShapeType.Sphere:
                {
                    return Collider3D.Create(new Sphere(worldCenter, shape.Radius));
                }
                default:
                {
                    float3 halfExtents = (float3)shape.Size * 0.5f;
                    return Collider3D.Create(new AABB(worldCenter, halfExtents));
                }
            }
        }
    }
}
#endif
