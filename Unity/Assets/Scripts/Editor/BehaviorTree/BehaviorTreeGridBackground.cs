using UnityEngine;
using UnityEngine.UIElements;

namespace ET
{
    public sealed class BehaviorTreeGridBackground : VisualElement
    {
        private static readonly Color BackgroundColor = new(0.13f, 0.13f, 0.13f, 1f);
        private static readonly Color MinorLineColor = new(0.22f, 0.22f, 0.22f, 0.75f);
        private static readonly Color MajorLineColor = new(0.32f, 0.32f, 0.32f, 0.95f);
        private const float MinorSpacing = 20f;
        private const float MajorSpacing = 100f;
        private readonly BehaviorTreeGraphView graphView;

        public BehaviorTreeGridBackground(BehaviorTreeGraphView graphView)
        {
            this.graphView = graphView;
            this.pickingMode = PickingMode.Ignore;
            this.style.backgroundColor = BackgroundColor;
            this.generateVisualContent += this.OnGenerateVisualContent;
        }

        private void OnGenerateVisualContent(MeshGenerationContext context)
        {
            Painter2D painter = context.painter2D;
            Rect rect = this.contentRect;
            if (rect.width <= 0 || rect.height <= 0)
            {
                return;
            }

            painter.lineWidth = 1f;
            Vector3 transformPosition = this.graphView?.viewTransform.position ?? Vector3.zero;
            float zoom = this.graphView?.viewTransform.scale.x ?? 1f;
            zoom = Mathf.Max(0.05f, zoom);

            DrawGrid(painter, rect, MinorSpacing, zoom, transformPosition, MinorLineColor, 6f, 6f);
            DrawGrid(painter, rect, MajorSpacing, zoom, transformPosition, MajorLineColor, 12f, 6f);
        }

        private static void DrawGrid(Painter2D painter, Rect rect, float baseSpacing, float zoom, Vector3 transformPosition, Color color, float dashLength, float gapLength)
        {
            float spacing = Mathf.Max(4f, baseSpacing * zoom);
            float verticalStart = Mathf.Repeat(transformPosition.x, spacing);
            float horizontalStart = Mathf.Repeat(transformPosition.y, spacing);

            painter.strokeColor = color;

            for (float x = verticalStart; x <= rect.width; x += spacing)
            {
                DrawDashedLine(painter,
                    new Vector2(rect.xMin + x, rect.yMin),
                    new Vector2(rect.xMin + x, rect.yMax),
                    dashLength,
                    gapLength,
                    zoom);
            }

            for (float y = horizontalStart; y <= rect.height; y += spacing)
            {
                DrawDashedLine(painter,
                    new Vector2(rect.xMin, rect.yMin + y),
                    new Vector2(rect.xMax, rect.yMin + y),
                    dashLength,
                    gapLength,
                    zoom);
            }
        }

        private static void DrawDashedLine(Painter2D painter, Vector2 start, Vector2 end, float baseDashLength, float baseGapLength, float zoom)
        {
            float dashLength = Mathf.Max(2f, baseDashLength * zoom);
            float gapLength = Mathf.Max(2f, baseGapLength * zoom);
            Vector2 direction = (end - start).normalized;
            float totalLength = Vector2.Distance(start, end);
            float distance = 0f;

            while (distance < totalLength)
            {
                Vector2 dashStart = start + direction * distance;
                Vector2 dashEnd = start + direction * Mathf.Min(distance + dashLength, totalLength);
                painter.BeginPath();
                painter.MoveTo(dashStart);
                painter.LineTo(dashEnd);
                painter.Stroke();
                distance += dashLength + gapLength;
            }
        }
    }
}
