using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;

[AddComponentMenu("UI/CircleImage")]
public class CircleImage : BaseImage
{
    // Use this for initialization
    void Awake()
    {
        innerVertices = new List<Vector3>();
        outterVertices = new List<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    [Range(0, 1)]
    public float scale = 1f;

    [Range(2, 30)]
    public int segements = 2;

    private List<Vector3> innerVertices;
    private List<Vector3> outterVertices;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        Rect pixelAdjustedRect = this.GetPixelAdjustedRect();

        Vector4 uv = overrideSprite != null ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;
        //Debug.LogError("uv:" + uv+","+uv.w+","+uv.x+","+uv.y+","+uv.z);

        Vector2 u = new Vector2(0, 0);
        float w = pixelAdjustedRect.width;

        #region
        //1
        for (int i = 0; i < segements + 1; i++)
        {
            UIVertex uivertex = new UIVertex();
            uivertex.color = color;
            if (i == 0)
            {
                //半径*
                uivertex.uv0 = new Vector2(uv.x + (scale / 2) * (uv.z - uv.x) * (1 - Mathf.Sin(i * 90 / segements)), uv.y + (scale / 2) * (uv.w - uv.y) * (1 - Mathf.Cos(i * 90 / segements)));
                u = new Vector2((scale / 2) * (1 - Mathf.Sin(i * 90 / segements)), (scale / 2) * (1 - Mathf.Cos(i * 90 / segements)));
            }
            else
            {
                uivertex.uv0 = new Vector2(uv.x + (scale / 2) * (uv.z - uv.x) * (1 - Mathf.Sin(Mathf.PI / (180f / (i * 90 / segements)))), uv.y + (scale / 2) * (uv.w - uv.y) * (1 - Mathf.Cos(Mathf.PI / (180f / (i * 90 / segements)))));
                u = new Vector2((scale / 2) * (1 - Mathf.Sin(Mathf.PI / (180f / (i * 90 / segements)))), (scale / 2) * (1 - Mathf.Cos(Mathf.PI / (180f / (i * 90 / segements)))));
            }
            uivertex.position = new Vector3(w * u.x - w / 2, w * u.y - w / 2);

            vh.AddVert(uivertex);
        }
        //2
        for (int i = 0; i < segements + 1; i++)
        {
            UIVertex uivertex = new UIVertex();
            uivertex.color = color;
            if (i == 0)
            {
                uivertex.uv0 = new Vector2(uv.x + (uv.z - uv.x) * (scale / 2) * (1 - Mathf.Cos(i * 90 / segements)), uv.y + (uv.w - uv.y) * ((1 - scale / 2) + (scale / 2) * Mathf.Sin(i * 90 / segements)));
                u = new Vector2((scale / 2) - (scale / 2) * Mathf.Cos(i * 90 / segements), (1 - scale / 2) + (scale / 2) * Mathf.Sin(i * 90 / segements));
            }
            else
            {
                uivertex.uv0 = new Vector2(uv.x + (uv.z - uv.x) * (scale / 2) * (1 - Mathf.Cos(Mathf.PI / (180f / (i * 90 / segements)))), uv.y + (uv.w - uv.y) * ((1 - scale / 2) + (scale / 2) * Mathf.Sin(Mathf.PI / (180f / (i * 90 / segements)))));
                u = new Vector2((scale / 2) * (1 - Mathf.Cos(Mathf.PI / (180f / (i * 90 / segements)))), (1 - scale / 2) + (scale / 2) * Mathf.Sin(Mathf.PI / (180f / (i * 90 / segements))));
            }
            uivertex.position = new Vector3(w * u.x - w / 2, w * u.y - w / 2);
            vh.AddVert(uivertex);
        }
        //3
        for (int i = 0; i < segements + 1; i++)
        {
            UIVertex uivertex = new UIVertex();
            uivertex.color = color;
            if (i == 0)
            {
                uivertex.uv0 = new Vector2(uv.x + (uv.z - uv.x) * ((1 - scale / 2) + (scale / 2) * Mathf.Sin(i * 90 / segements)), uv.y + (uv.w - uv.y) * ((1 - scale / 2) + (scale / 2) * Mathf.Cos(i * 90 / segements)));
                u = new Vector2((1 - scale / 2) + (scale / 2) * Mathf.Sin(i * 90 / segements), (1 - scale / 2) + (scale / 2) * Mathf.Cos(i * 90 / segements));
            }
            else
            {
                uivertex.uv0 = new Vector2(uv.x + (uv.z - uv.x) * ((1 - scale / 2) + (scale / 2) * Mathf.Sin(Mathf.PI / (180f / (i * 90 / segements)))), uv.y + (uv.w - uv.y) * ((1 - scale / 2) + (scale / 2) * Mathf.Cos(Mathf.PI / (180f / (i * 90 / segements)))));
                u = new Vector2((1 - scale / 2) + (scale / 2) * Mathf.Sin(Mathf.PI / (180f / (i * 90 / segements))), (1 - scale / 2) + (scale / 2) * Mathf.Cos(Mathf.PI / (180f / (i * 90 / segements))));
            }
            uivertex.position = new Vector3(w * u.x - w / 2, w * u.y - w / 2);
            vh.AddVert(uivertex);
        }
        //4
        for (int i = 0; i < segements + 1; i++)
        {
            UIVertex uivertex = new UIVertex();
            uivertex.color = color;
            if (i == 0)
            {
                uivertex.uv0 = new Vector2(uv.x + (uv.z - uv.x) * ((1 - scale / 2) + (scale / 2) * Mathf.Cos(i * 90 / segements)), uv.y + (uv.w - uv.y) * ((scale / 2) - (scale / 2) * Mathf.Sin(i * 90 / segements)));
                u = new Vector2((1 - scale / 2) + (scale / 2) * Mathf.Cos(i * 90 / segements), (scale / 2) - (scale / 2) * Mathf.Sin(i * 90 / segements));
            }
            else
            {
                uivertex.uv0 = new Vector2(uv.x + (uv.z - uv.x) * ((1 - scale / 2) + (scale / 2) * Mathf.Cos(Mathf.PI / (180f / (i * 90 / segements)))), uv.y + (uv.w - uv.y) * ((scale / 2) - (scale / 2) * Mathf.Sin(Mathf.PI / (180f / (i * 90 / segements)))));
                u = new Vector2((1 - scale / 2) + (scale / 2) * Mathf.Cos(Mathf.PI / (180f / (i * 90 / segements))), (scale / 2) - (scale / 2) * Mathf.Sin(Mathf.PI / (180f / (i * 90 / segements))));
            }
            uivertex.position = new Vector3(w * u.x - w / 2, w * u.y - w / 2);
            vh.AddVert(uivertex);
        }

        //（（点*4+8个点-3）条弦+1）个三角形
        for (int i = 0; i < ((segements - 1) * 4 + 8 - 3 + 1); i++)
        {
            vh.AddTriangle(0, i + 1, i + 2);
        }
        #endregion
    }

    public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        //        Sprite sprite = overrideSprite;
        //        if (sprite == null)
        //            return true;


        //    Debug.LogError("鼠标点击:" + screenPoint);


        //        Vector2 local;
        //        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out local);
        return true;
    }

    private bool Contains(Vector2 p, List<Vector3> outterVertices, List<Vector3> innerVertices)
    {
        var crossNumber = 0;
        RayCrossing(p, innerVertices, ref crossNumber);//检测内环
        RayCrossing(p, outterVertices, ref crossNumber);//检测外环
        return (crossNumber & 1) == 1;
    }

    /// <summary>
    /// 使用RayCrossing算法判断点击点是否在封闭多边形里
    /// </summary>
    /// <param name="p"></param>
    /// <param name="vertices"></param>
    /// <param name="crossNumber"></param>
    private void RayCrossing(Vector2 p, List<Vector3> vertices, ref int crossNumber)
    {
        for (int i = 0, count = vertices.Count; i < count; i++)
        {
            var v1 = vertices[i];
            var v2 = vertices[(i + 1) % count];

            //点击点水平线必须与两顶点线段相交
            if (((v1.y <= p.y) && (v2.y > p.y))
                || ((v1.y > p.y) && (v2.y <= p.y)))
            {
                //只考虑点击点右侧方向，点击点水平线与线段相交，且交点x > 点击点x，则crossNumber+1
                if (p.x < v1.x + (p.y - v1.y) / (v2.y - v1.y) * (v2.x - v1.x))
                {
                    crossNumber += 1;
                }
            }
        }
    }

}