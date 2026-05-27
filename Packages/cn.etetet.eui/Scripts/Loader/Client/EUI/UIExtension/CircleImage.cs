using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace ET.Client
{
    /// <summary>
    /// 圆形 Image：基于 BaseImage 通过四象限分段绘制圆形 mesh。
    /// </summary>
    [AddComponentMenu("UI/CircleImage")]
    public class CircleImage : BaseImage
    {
        [Range(0, 1)]
        public float scale = 1f;

        [Range(2, 30)]
        public int segements = 2;

        private List<Vector3> innerVertices;
        private List<Vector3> outterVertices;

        protected override void Awake()
        {
            base.Awake();
            innerVertices = new List<Vector3>();
            outterVertices = new List<Vector3>();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            Rect pixelAdjustedRect = this.GetPixelAdjustedRect();

            Vector4 uv = overrideSprite != null ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;
            Vector2 u;
            float w = pixelAdjustedRect.width;

            // 四象限分段绘制
            for (int i = 0; i < segements + 1; i++)
            {
                UIVertex uivertex = new UIVertex { color = color };
                if (i == 0)
                {
                    uivertex.uv0 = new Vector2(uv.x + (scale / 2) * (uv.z - uv.x) * (1 - Mathf.Sin(i * 90f / segements)),
                                                uv.y + (scale / 2) * (uv.w - uv.y) * (1 - Mathf.Cos(i * 90f / segements)));
                    u = new Vector2((scale / 2) * (1 - Mathf.Sin(i * 90f / segements)),
                                    (scale / 2) * (1 - Mathf.Cos(i * 90f / segements)));
                }
                else
                {
                    uivertex.uv0 = new Vector2(uv.x + (scale / 2) * (uv.z - uv.x) * (1 - Mathf.Sin(Mathf.PI / (180f / (i * 90f / segements)))),
                                                uv.y + (scale / 2) * (uv.w - uv.y) * (1 - Mathf.Cos(Mathf.PI / (180f / (i * 90f / segements)))));
                    u = new Vector2((scale / 2) * (1 - Mathf.Sin(Mathf.PI / (180f / (i * 90f / segements)))),
                                    (scale / 2) * (1 - Mathf.Cos(Mathf.PI / (180f / (i * 90f / segements)))));
                }
                uivertex.position = new Vector3(w * u.x - w / 2, w * u.y - w / 2);
                vh.AddVert(uivertex);
            }
            for (int i = 0; i < segements + 1; i++)
            {
                UIVertex uivertex = new UIVertex { color = color };
                if (i == 0)
                {
                    uivertex.uv0 = new Vector2(uv.x + (uv.z - uv.x) * (scale / 2) * (1 - Mathf.Cos(i * 90f / segements)),
                                                uv.y + (uv.w - uv.y) * ((1 - scale / 2) + (scale / 2) * Mathf.Sin(i * 90f / segements)));
                    u = new Vector2((scale / 2) - (scale / 2) * Mathf.Cos(i * 90f / segements),
                                    (1 - scale / 2) + (scale / 2) * Mathf.Sin(i * 90f / segements));
                }
                else
                {
                    uivertex.uv0 = new Vector2(uv.x + (uv.z - uv.x) * (scale / 2) * (1 - Mathf.Cos(Mathf.PI / (180f / (i * 90f / segements)))),
                                                uv.y + (uv.w - uv.y) * ((1 - scale / 2) + (scale / 2) * Mathf.Sin(Mathf.PI / (180f / (i * 90f / segements)))));
                    u = new Vector2((scale / 2) * (1 - Mathf.Cos(Mathf.PI / (180f / (i * 90f / segements)))),
                                    (1 - scale / 2) + (scale / 2) * Mathf.Sin(Mathf.PI / (180f / (i * 90f / segements))));
                }
                uivertex.position = new Vector3(w * u.x - w / 2, w * u.y - w / 2);
                vh.AddVert(uivertex);
            }
            for (int i = 0; i < segements + 1; i++)
            {
                UIVertex uivertex = new UIVertex { color = color };
                if (i == 0)
                {
                    uivertex.uv0 = new Vector2(uv.x + (uv.z - uv.x) * ((1 - scale / 2) + (scale / 2) * Mathf.Sin(i * 90f / segements)),
                                                uv.y + (uv.w - uv.y) * ((1 - scale / 2) + (scale / 2) * Mathf.Cos(i * 90f / segements)));
                    u = new Vector2((1 - scale / 2) + (scale / 2) * Mathf.Sin(i * 90f / segements),
                                    (1 - scale / 2) + (scale / 2) * Mathf.Cos(i * 90f / segements));
                }
                else
                {
                    uivertex.uv0 = new Vector2(uv.x + (uv.z - uv.x) * ((1 - scale / 2) + (scale / 2) * Mathf.Sin(Mathf.PI / (180f / (i * 90f / segements)))),
                                                uv.y + (uv.w - uv.y) * ((1 - scale / 2) + (scale / 2) * Mathf.Cos(Mathf.PI / (180f / (i * 90f / segements)))));
                    u = new Vector2((1 - scale / 2) + (scale / 2) * Mathf.Sin(Mathf.PI / (180f / (i * 90f / segements))),
                                    (1 - scale / 2) + (scale / 2) * Mathf.Cos(Mathf.PI / (180f / (i * 90f / segements))));
                }
                uivertex.position = new Vector3(w * u.x - w / 2, w * u.y - w / 2);
                vh.AddVert(uivertex);
            }
            for (int i = 0; i < segements + 1; i++)
            {
                UIVertex uivertex = new UIVertex { color = color };
                if (i == 0)
                {
                    uivertex.uv0 = new Vector2(uv.x + (uv.z - uv.x) * ((1 - scale / 2) + (scale / 2) * Mathf.Cos(i * 90f / segements)),
                                                uv.y + (uv.w - uv.y) * ((scale / 2) - (scale / 2) * Mathf.Sin(i * 90f / segements)));
                    u = new Vector2((1 - scale / 2) + (scale / 2) * Mathf.Cos(i * 90f / segements),
                                    (scale / 2) - (scale / 2) * Mathf.Sin(i * 90f / segements));
                }
                else
                {
                    uivertex.uv0 = new Vector2(uv.x + (uv.z - uv.x) * ((1 - scale / 2) + (scale / 2) * Mathf.Cos(Mathf.PI / (180f / (i * 90f / segements)))),
                                                uv.y + (uv.w - uv.y) * ((scale / 2) - (scale / 2) * Mathf.Sin(Mathf.PI / (180f / (i * 90f / segements)))));
                    u = new Vector2((1 - scale / 2) + (scale / 2) * Mathf.Cos(Mathf.PI / (180f / (i * 90f / segements))),
                                    (scale / 2) - (scale / 2) * Mathf.Sin(Mathf.PI / (180f / (i * 90f / segements))));
                }
                uivertex.position = new Vector3(w * u.x - w / 2, w * u.y - w / 2);
                vh.AddVert(uivertex);
            }

            // 三角形索引
            for (int i = 0; i < ((segements - 1) * 4 + 8 - 3 + 1); i++)
            {
                vh.AddTriangle(0, i + 1, i + 2);
            }
        }

        public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            return true;
        }
    }
}
