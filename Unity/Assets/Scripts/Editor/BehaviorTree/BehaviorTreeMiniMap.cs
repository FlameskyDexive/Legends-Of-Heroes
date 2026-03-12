using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace ET
{
    public sealed class BehaviorTreeMiniMap : VisualElement
    {
        private enum HoverArea
        {
            None = 0,
            Border = 1,
            Content = 2,
        }

        private static readonly Color HeaderColor = new(0.18f, 0.18f, 0.18f, 1f);
        private static readonly Color HeaderHoverColor = new(0.24f, 0.24f, 0.24f, 1f);
        private static readonly Color BackgroundColor = new(0.10f, 0.10f, 0.10f, 0.96f);
        private static readonly Color BorderColor = new(0.35f, 0.35f, 0.35f, 1f);
        private static readonly Color BorderHoverColor = new(0.55f, 0.55f, 0.55f, 1f);
        private static readonly Color ContentBorderColor = new(0.24f, 0.24f, 0.24f, 1f);
        private static readonly Color ContentHoverBorderColor = new(0.46f, 0.46f, 0.46f, 1f);
        private static readonly Color ViewportColor = new(1f, 1f, 1f, 1f);
        private readonly BehaviorTreeGraphView graphView;
        private readonly Label titleLabel;
        private bool isDraggingBorder;
        private bool isDraggingContent;
        private Vector2 dragStartMouseInParent;
        private Rect dragStartRect;
        private HoverArea hoverArea;

        private const float BorderThickness = 10f;
        private const float HeaderHeight = 20f;

        public BehaviorTreeMiniMap(BehaviorTreeGraphView graphView)
        {
            this.graphView = graphView;
            this.style.position = Position.Absolute;
            this.style.width = 220;
            this.style.height = 140;
            this.style.backgroundColor = BackgroundColor;
            this.style.borderLeftWidth = 1;
            this.style.borderRightWidth = 1;
            this.style.borderTopWidth = 1;
            this.style.borderBottomWidth = 1;
            this.style.borderLeftColor = BorderColor;
            this.style.borderRightColor = BorderColor;
            this.style.borderTopColor = BorderColor;
            this.style.borderBottomColor = BorderColor;
            this.style.unityOverflowClipBox = OverflowClipBox.ContentBox;

            this.titleLabel = new Label("MiniMap");
            this.titleLabel.style.position = Position.Absolute;
            this.titleLabel.style.left = 0;
            this.titleLabel.style.right = 0;
            this.titleLabel.style.top = 0;
            this.titleLabel.style.height = HeaderHeight;
            this.titleLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            this.titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            this.titleLabel.style.backgroundColor = HeaderColor;
            this.titleLabel.style.color = new Color(0.9f, 0.9f, 0.9f, 1f);
            this.titleLabel.pickingMode = PickingMode.Ignore;
            this.Add(this.titleLabel);

            this.RegisterCallback<MouseDownEvent>(this.OnMouseDownEvent, TrickleDown.TrickleDown);
            this.RegisterCallback<MouseMoveEvent>(this.OnMouseMoveEvent, TrickleDown.TrickleDown);
            this.RegisterCallback<MouseUpEvent>(this.OnMouseUpEvent, TrickleDown.TrickleDown);
            this.RegisterCallback<MouseLeaveEvent>(this.OnMouseLeaveEvent, TrickleDown.TrickleDown);
            this.RegisterCallback<WheelEvent>(this.OnWheelEvent, TrickleDown.TrickleDown);
            this.generateVisualContent += this.OnGenerateVisualContent;
        }

        public void SetRect(Rect rect)
        {
            this.style.left = rect.x;
            this.style.top = rect.y;
            this.style.width = rect.width;
            this.style.height = rect.height;
        }

        private void OnGenerateVisualContent(MeshGenerationContext context)
        {
            Painter2D painter = context.painter2D;
            Rect rect = this.contentRect;
            if (rect.width <= 0 || rect.height <= 0)
            {
                return;
            }

            Rect innerRect = this.GetInnerContentRect(rect);
            Color outerBorderColor = this.hoverArea == HoverArea.Border ? BorderHoverColor : BorderColor;
            Color headerColor = this.hoverArea == HoverArea.Border ? HeaderHoverColor : HeaderColor;
            Color innerBorderColor = this.hoverArea == HoverArea.Content ? ContentHoverBorderColor : ContentBorderColor;

            DrawRect(painter, rect, outerBorderColor, 1f);
            FillRect(painter, new Rect(rect.xMin, rect.yMin, rect.width, HeaderHeight), headerColor);
            DrawRect(painter, innerRect, innerBorderColor, 1f);

            Rect worldBounds = this.GetContentWorldBounds();
            if (worldBounds.width <= 0 || worldBounds.height <= 0)
            {
                return;
            }

            this.DrawConnections(painter, worldBounds, innerRect);

            foreach (BehaviorTreeNodeView nodeView in this.graphView.GetNodeViews())
            {
                Rect nodeRect = nodeView.GetPosition();
                Rect miniRect = this.MapRect(nodeRect, worldBounds, innerRect);
                miniRect.width = Mathf.Max(2f, miniRect.width);
                miniRect.height = Mathf.Max(2f, miniRect.height);

                Color fillColor = BehaviorTreeEditorUtility.GetNodeHeaderColor(nodeView.Data.NodeKind, BehaviorTreeNodeState.Inactive);
                fillColor.a = 0.85f;
                FillRect(painter, miniRect, fillColor);
            }

            Rect viewportRect = this.GetViewportContentRect();
            Rect miniViewport = this.MapRect(viewportRect, worldBounds, innerRect);
            DrawDashedRect(painter, miniViewport, ViewportColor, 6f, 4f);
        }

        private void OnMouseDownEvent(MouseDownEvent evt)
        {
            if (evt.button != 0)
            {
                return;
            }

            Rect outerRect = this.contentRect;
            Rect innerRect = this.GetInnerContentRect(outerRect);
            this.UpdateHoverArea(evt.localMousePosition, outerRect, innerRect);
            this.dragStartMouseInParent = this.parent?.WorldToLocal(evt.mousePosition) ?? evt.localMousePosition;
            this.dragStartRect = this.layout;

            Rect headerRect = new Rect(0f, 0f, outerRect.width, HeaderHeight);

            if (innerRect.Contains(evt.localMousePosition))
            {
                this.isDraggingContent = true;
                this.CaptureMouse();
                this.MoveViewportTo(evt.localMousePosition);
                evt.StopImmediatePropagation();
                return;
            }

            if (headerRect.Contains(evt.localMousePosition) || outerRect.Contains(evt.localMousePosition))
            {
                this.isDraggingBorder = true;
                this.CaptureMouse();
                evt.StopImmediatePropagation();
            }
        }

        private void OnMouseMoveEvent(MouseMoveEvent evt)
        {
            if (this.isDraggingBorder)
            {
                Vector2 currentMouseInParent = this.parent?.WorldToLocal(evt.mousePosition) ?? evt.localMousePosition;
                Vector2 delta = currentMouseInParent - this.dragStartMouseInParent;
                Rect rect = this.ClampToParent(new Rect(this.dragStartRect.x + delta.x, this.dragStartRect.y + delta.y, this.dragStartRect.width, this.dragStartRect.height));
                this.SetRect(rect);
                evt.StopImmediatePropagation();
                return;
            }

            if (this.isDraggingContent)
            {
                this.MoveViewportTo(evt.localMousePosition);
                evt.StopImmediatePropagation();
                return;
            }

            this.UpdateHoverArea(evt.localMousePosition, this.contentRect, this.GetInnerContentRect(this.contentRect));
        }

        private void OnMouseUpEvent(MouseUpEvent evt)
        {
            if (evt.button != 0)
            {
                return;
            }

            this.StopDragging();
        }

        private void OnMouseLeaveEvent(MouseLeaveEvent evt)
        {
            this.SetHoverArea(HoverArea.None);
            if (!this.HasMouseCapture())
            {
                return;
            }

            this.StopDragging();
        }

        private void OnWheelEvent(WheelEvent evt)
        {
            float scaleDelta = evt.delta.y > 0 ? 0.9f : 1.1f;
            float currentScale = Mathf.Max(0.05f, this.graphView.viewTransform.scale.x);
            float newScaleValue = Mathf.Clamp(currentScale * scaleDelta, 0.2f, 2.0f);
            Vector2 viewportCenter = new(this.graphView.layout.width * 0.5f, this.graphView.layout.height * 0.5f);
            Vector2 currentViewportCenterInContent = this.GetViewportContentRect().center;

            Vector3 position = new(
                viewportCenter.x - currentViewportCenterInContent.x * newScaleValue,
                viewportCenter.y - currentViewportCenterInContent.y * newScaleValue,
                0f);

            this.graphView.UpdateViewTransform(position, new Vector3(newScaleValue, newScaleValue, 1f));
            this.MarkDirtyRepaint();
            evt.StopImmediatePropagation();
        }

        private void StopDragging()
        {
            bool wasDraggingBorder = this.isDraggingBorder;
            this.isDraggingBorder = false;
            this.isDraggingContent = false;
            if (this.HasMouseCapture())
            {
                this.ReleaseMouse();
            }

            if (wasDraggingBorder)
            {
                this.graphView?.GetWindow()?.SaveMiniMapRect(this.layout);
            }
        }

        private void UpdateHoverArea(Vector2 localPosition, Rect outerRect, Rect innerRect)
        {
            Rect headerRect = new Rect(0f, 0f, outerRect.width, HeaderHeight);
            if (innerRect.Contains(localPosition))
            {
                this.SetHoverArea(HoverArea.Content);
                return;
            }

            if (headerRect.Contains(localPosition) || outerRect.Contains(localPosition))
            {
                this.SetHoverArea(HoverArea.Border);
                return;
            }

            this.SetHoverArea(HoverArea.None);
        }

        private void SetHoverArea(HoverArea area)
        {
            if (this.hoverArea == area)
            {
                return;
            }

            this.hoverArea = area;
            this.MarkDirtyRepaint();
        }

        private void MoveViewportTo(Vector2 localPosition)
        {
            Rect outerRect = this.contentRect;
            Rect innerRect = this.GetInnerContentRect(outerRect);
            Rect worldBounds = this.GetContentWorldBounds();
            if (worldBounds.width <= 0 || worldBounds.height <= 0)
            {
                return;
            }

            float normalizedX = Mathf.InverseLerp(innerRect.xMin, innerRect.xMax, localPosition.x);
            float normalizedY = Mathf.InverseLerp(innerRect.yMin, innerRect.yMax, localPosition.y);
            Vector2 contentPoint = new(
                Mathf.Lerp(worldBounds.xMin, worldBounds.xMax, normalizedX),
                Mathf.Lerp(worldBounds.yMin, worldBounds.yMax, normalizedY));

            Vector3 scale = this.graphView.viewTransform.scale;
            Vector2 viewportCenter = new(this.graphView.layout.width * 0.5f, this.graphView.layout.height * 0.5f);
            Vector3 position = new(
                viewportCenter.x - contentPoint.x * scale.x,
                viewportCenter.y - contentPoint.y * scale.y,
                0f);

            this.graphView.UpdateViewTransform(position, scale);
            this.MarkDirtyRepaint();
        }

        private Rect GetViewportContentRect()
        {
            Vector2 topLeft = this.graphView.contentViewContainer.WorldToLocal(new Vector2(this.graphView.worldBound.xMin, this.graphView.worldBound.yMin));
            Vector2 bottomRight = this.graphView.contentViewContainer.WorldToLocal(new Vector2(this.graphView.worldBound.xMax, this.graphView.worldBound.yMax));
            return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
        }

        private void DrawConnections(Painter2D painter, Rect worldBounds, Rect innerRect)
        {
            painter.strokeColor = new Color(0.75f, 0.75f, 0.78f, 0.65f);
            painter.lineWidth = 1f;

            foreach (var edge in this.graphView.edges.ToList())
            {
                if (edge.output?.node is not BehaviorTreeNodeView outputNodeView || edge.input?.node is not BehaviorTreeNodeView inputNodeView)
                {
                    continue;
                }

                Rect outputRect = outputNodeView.GetPosition();
                Rect inputRect = inputNodeView.GetPosition();
                Vector2 from = this.MapPoint(new Vector2(outputRect.center.x, outputRect.yMax), worldBounds, innerRect);
                Vector2 to = this.MapPoint(new Vector2(inputRect.center.x, inputRect.yMin), worldBounds, innerRect);

                switch (this.graphView.GetConnectionStyle())
                {
                    case BehaviorTreeConnectionStyle.Straight:
                        DrawLine(painter, from, to);
                        break;
                    case BehaviorTreeConnectionStyle.Curve:
                        DrawBezier(painter, from, to);
                        break;
                    default:
                        DrawOrthogonal(painter, from, to);
                        break;
                }
            }
        }

        private Rect GetContentWorldBounds()
        {
            List<Rect> rects = this.graphView.GetNodeViews().Select(nodeView => nodeView.GetPosition()).ToList();
            rects.Add(this.GetViewportContentRect());
            if (rects.Count == 0)
            {
                return new Rect(0f, 0f, 1000f, 1000f);
            }

            float minX = rects.Min(rect => rect.xMin);
            float minY = rects.Min(rect => rect.yMin);
            float maxX = rects.Max(rect => rect.xMax);
            float maxY = rects.Max(rect => rect.yMax);
            const float padding = 80f;
            return Rect.MinMaxRect(minX - padding, minY - padding, maxX + padding, maxY + padding);
        }

        private Rect MapRect(Rect sourceRect, Rect sourceBounds, Rect targetBounds)
        {
            float xMin = Mathf.Lerp(targetBounds.xMin, targetBounds.xMax, Mathf.InverseLerp(sourceBounds.xMin, sourceBounds.xMax, sourceRect.xMin));
            float xMax = Mathf.Lerp(targetBounds.xMin, targetBounds.xMax, Mathf.InverseLerp(sourceBounds.xMin, sourceBounds.xMax, sourceRect.xMax));
            float yMin = Mathf.Lerp(targetBounds.yMin, targetBounds.yMax, Mathf.InverseLerp(sourceBounds.yMin, sourceBounds.yMax, sourceRect.yMin));
            float yMax = Mathf.Lerp(targetBounds.yMin, targetBounds.yMax, Mathf.InverseLerp(sourceBounds.yMin, sourceBounds.yMax, sourceRect.yMax));
            return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        }

        private Vector2 MapPoint(Vector2 sourcePoint, Rect sourceBounds, Rect targetBounds)
        {
            return new Vector2(
                Mathf.Lerp(targetBounds.xMin, targetBounds.xMax, Mathf.InverseLerp(sourceBounds.xMin, sourceBounds.xMax, sourcePoint.x)),
                Mathf.Lerp(targetBounds.yMin, targetBounds.yMax, Mathf.InverseLerp(sourceBounds.yMin, sourceBounds.yMax, sourcePoint.y)));
        }

        private Rect GetInnerContentRect(Rect outerRect)
        {
            return new Rect(
                outerRect.xMin + BorderThickness,
                outerRect.yMin + HeaderHeight + BorderThickness,
                Mathf.Max(10f, outerRect.width - BorderThickness * 2f),
                Mathf.Max(10f, outerRect.height - HeaderHeight - BorderThickness * 2f));
        }

        private Rect ClampToParent(Rect rect)
        {
            if (this.parent == null)
            {
                return rect;
            }

            Rect bounds = this.parent.contentRect;
            float clampedX = Mathf.Clamp(rect.x, bounds.xMin, Mathf.Max(bounds.xMin, bounds.xMax - rect.width));
            float clampedY = Mathf.Clamp(rect.y, bounds.yMin, Mathf.Max(bounds.yMin, bounds.yMax - rect.height));
            return new Rect(clampedX, clampedY, rect.width, rect.height);
        }

        private static void FillRect(Painter2D painter, Rect rect, Color color)
        {
            painter.fillColor = color;
            painter.BeginPath();
            painter.MoveTo(new Vector2(rect.xMin, rect.yMin));
            painter.LineTo(new Vector2(rect.xMax, rect.yMin));
            painter.LineTo(new Vector2(rect.xMax, rect.yMax));
            painter.LineTo(new Vector2(rect.xMin, rect.yMax));
            painter.ClosePath();
            painter.Fill();
        }

        private static void DrawRect(Painter2D painter, Rect rect, Color color, float width)
        {
            painter.strokeColor = color;
            painter.lineWidth = width;
            painter.BeginPath();
            painter.MoveTo(new Vector2(rect.xMin, rect.yMin));
            painter.LineTo(new Vector2(rect.xMax, rect.yMin));
            painter.LineTo(new Vector2(rect.xMax, rect.yMax));
            painter.LineTo(new Vector2(rect.xMin, rect.yMax));
            painter.ClosePath();
            painter.Stroke();
        }

        private static void DrawLine(Painter2D painter, Vector2 from, Vector2 to)
        {
            painter.BeginPath();
            painter.MoveTo(from);
            painter.LineTo(to);
            painter.Stroke();
        }

        private static void DrawOrthogonal(Painter2D painter, Vector2 from, Vector2 to)
        {
            float middleY = (from.y + to.y) * 0.5f;
            painter.BeginPath();
            painter.MoveTo(from);
            painter.LineTo(new Vector2(from.x, middleY));
            painter.LineTo(new Vector2(to.x, middleY));
            painter.LineTo(to);
            painter.Stroke();
        }

        private static void DrawBezier(Painter2D painter, Vector2 from, Vector2 to)
        {
            float tangent = Mathf.Max(10f, Mathf.Abs(to.y - from.y) * 0.5f);
            Vector2 fromTangent = new(from.x, from.y + tangent);
            Vector2 toTangent = new(to.x, to.y - tangent);
            painter.BeginPath();
            painter.MoveTo(from);
            painter.BezierCurveTo(fromTangent, toTangent, to);
            painter.Stroke();
        }

        private static void DrawDashedRect(Painter2D painter, Rect rect, Color color, float dashLength, float gapLength)
        {
            painter.strokeColor = color;
            painter.lineWidth = 1f;
            DrawDashedLine(painter, new Vector2(rect.xMin, rect.yMin), new Vector2(rect.xMax, rect.yMin), dashLength, gapLength);
            DrawDashedLine(painter, new Vector2(rect.xMax, rect.yMin), new Vector2(rect.xMax, rect.yMax), dashLength, gapLength);
            DrawDashedLine(painter, new Vector2(rect.xMax, rect.yMax), new Vector2(rect.xMin, rect.yMax), dashLength, gapLength);
            DrawDashedLine(painter, new Vector2(rect.xMin, rect.yMax), new Vector2(rect.xMin, rect.yMin), dashLength, gapLength);
        }

        private static void DrawDashedLine(Painter2D painter, Vector2 start, Vector2 end, float dashLength, float gapLength)
        {
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
