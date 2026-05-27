using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    /// <summary>
    /// Text 字间距 BaseMeshEffect：在 Text mesh 上为每个字符加上水平偏移以扩大字间距。
    /// 与 Text 组件配合使用，挂在同一 GameObject 上。
    /// </summary>
    [AddComponentMenu("UI/Effects/TextSpacing")]
    public class TextSpacing : BaseMeshEffect
    {
        public float _textSpacing = 1f;

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive() || vh.currentVertCount == 0)
            {
                return;
            }

            Text text = GetComponent<Text>();
            if (text == null)
            {
                Debug.LogError("Missing Text component");
                return;
            }

            List<UIVertex> vertexs = new List<UIVertex>();
            vh.GetUIVertexStream(vertexs);

            string[] lineTexts = text.text.Split('\n');
            Line[] lines = new Line[lineTexts.Length];

            // 按每个字符占 6 个顶点的规律计算每一行的顶点起止
            for (int i = 0; i < lines.Length; i++)
            {
                if (i == 0)
                {
                    lines[i] = new Line(0, lineTexts[i].Length + 1);
                }
                else if (i > 0 && i < lines.Length - 1)
                {
                    lines[i] = new Line(lines[i - 1].EndVertexIndex + 1, lineTexts[i].Length + 1);
                }
                else
                {
                    lines[i] = new Line(lines[i - 1].EndVertexIndex + 1, lineTexts[i].Length);
                }
            }

            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = lines[i].StartVertexIndex + 6; j <= lines[i].EndVertexIndex; j++)
                {
                    if (j < 0 || j >= vertexs.Count)
                    {
                        continue;
                    }
                    UIVertex vt = vertexs[j];
                    vt.position += new Vector3(_textSpacing * ((j - lines[i].StartVertexIndex) / 6), 0, 0);
                    vertexs[j] = vt;
                    if (j % 6 <= 2)
                    {
                        vh.SetUIVertex(vt, (j / 6) * 4 + j % 6);
                    }
                    if (j % 6 == 4)
                    {
                        vh.SetUIVertex(vt, (j / 6) * 4 + j % 6 - 1);
                    }
                }
            }
        }
    }
}
