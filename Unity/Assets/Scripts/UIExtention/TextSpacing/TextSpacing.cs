using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        int indexCount = vh.currentIndexCount;

        string[] lineTexts = text.text.Split('\n');

        Line[] lines = new Line[lineTexts.Length];

        //根据lines数组中各个元素的长度计算每一行中第一个点的索引，每个字、字母、空母均占6个点
        for (int i = 0; i < lines.Length; i++)
        {
            //除最后一行外，vertexs对于前面几行都有回车符占了6个点
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

        UIVertex vt;

        for (int i = 0; i < lines.Length; i++)
        {
            for (int j = lines[i].StartVertexIndex + 6; j <= lines[i].EndVertexIndex; j++)
            {
                if (j < 0 || j >= vertexs.Count)
                {
                    continue;
                }
                vt = vertexs[j];
                vt.position += new Vector3(_textSpacing * ((j - lines[i].StartVertexIndex) / 6), 0, 0);
                vertexs[j] = vt;
                //以下注意点与索引的对应关系
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