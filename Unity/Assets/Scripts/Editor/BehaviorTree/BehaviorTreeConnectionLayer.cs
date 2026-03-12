using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ET
{
    public enum BehaviorTreeConnectionStyle
    {
        Straight = 0,
        Orthogonal = 1,
        Curve = 2,
    }

    public sealed class BehaviorTreeConnectionLayer : VisualElement
    {
        private static readonly Color ConnectionColor = new(0.86f, 0.86f, 0.86f, 0.95f);
        private readonly BehaviorTreeGraphView graphView;
        private BehaviorTreeConnectionStyle connectionStyle = BehaviorTreeConnectionStyle.Orthogonal;

        public BehaviorTreeConnectionLayer(BehaviorTreeGraphView graphView)
        {
            this.graphView = graphView;
            this.pickingMode = PickingMode.Ignore;
            this.style.position = Position.Absolute;
            this.style.left = 0;
            this.style.top = 0;
            this.style.right = 0;
            this.style.bottom = 0;
            this.StretchToParentSize();
            this.generateVisualContent += this.OnGenerateVisualContent;
        }

        public void SetConnectionStyle(BehaviorTreeConnectionStyle style)
        {
            this.connectionStyle = style;
            this.MarkDirtyRepaint();
        }

        private void OnGenerateVisualContent(MeshGenerationContext context)
        {
            Painter2D painter = context.painter2D;
            painter.lineWidth = 2f;
            painter.strokeColor = ConnectionColor;

            foreach (Edge edge in this.graphView.edges.ToList())
            {
                if (edge.output == null || edge.input == null)
                {
                    continue;
                }

                if (edge.output.node is not BehaviorTreeNodeView outputNodeView || edge.input.node is not BehaviorTreeNodeView inputNodeView)
                {
                    continue;
                }

                Vector2 from = outputNodeView.GetOutputAnchorContentPosition();
                Vector2 to = inputNodeView.GetInputAnchorContentPosition();
                if (this.connectionStyle == BehaviorTreeConnectionStyle.Straight)
                {
                    DrawStraightLine(painter, from, to);
                }
                else if (this.connectionStyle == BehaviorTreeConnectionStyle.Curve)
                {
                    DrawBezierLine(painter, from, to);
                }
                else
                {
                    DrawOrthogonalLine(painter, from, to);
                }
            }
        }

        private static void DrawStraightLine(Painter2D painter, Vector2 from, Vector2 to)
        {
            painter.BeginPath();
            painter.MoveTo(from);
            painter.LineTo(to);
            painter.Stroke();
        }

        private static void DrawBezierLine(Painter2D painter, Vector2 from, Vector2 to)
        {
            float tangent = Mathf.Max(40f, Mathf.Abs(to.y - from.y) * 0.5f);
            Vector2 fromTangent = new(from.x, from.y + tangent);
            Vector2 toTangent = new(to.x, to.y - tangent);

            painter.BeginPath();
            painter.MoveTo(from);
            painter.BezierCurveTo(fromTangent, toTangent, to);
            painter.Stroke();
        }

        private static void DrawOrthogonalLine(Painter2D painter, Vector2 from, Vector2 to)
        {
            float middleY = (from.y + to.y) * 0.5f;

            painter.BeginPath();
            painter.MoveTo(from);
            painter.LineTo(new Vector2(from.x, middleY));
            painter.LineTo(new Vector2(to.x, middleY));
            painter.LineTo(to);
            painter.Stroke();
        }
    }
}
