using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ET
{
    public sealed class BehaviorTreeGraphView : GraphView
    {
        private sealed class ClipboardSelection
        {
            public readonly List<BehaviorTreeEditorNodeData> Nodes = new();
            public readonly List<(string ParentId, string ChildId)> Connections = new();
        }

        private static ClipboardSelection clipboard;
        private readonly BehaviorTreeConnectionLayer connectionLayer;
        private readonly Dictionary<string, BehaviorTreeNodeView> nodeViews = new();
        private readonly BehaviorTreeEditorWindow window;
        private readonly BehaviorTreeGridBackground gridBackground;
        private readonly BehaviorTreeSearchWindowProvider searchWindowProvider;
        private bool isPopulating;
        private BehaviorTreeMiniMap miniMap;
        private Vector2 pendingNodeCreationContentPosition;
        private bool hasPendingNodeCreationPosition;
        private BehaviorTreeConnectionStyle connectionStyle = BehaviorTreeConnectionStyle.Orthogonal;

        public BehaviorTreeGraphView(BehaviorTreeEditorWindow window)
        {
            this.window = window;
            this.style.flexGrow = 1;
            this.style.backgroundColor = new Color(0.12f, 0.12f, 0.12f);

            this.gridBackground = new BehaviorTreeGridBackground(this);
            this.gridBackground.pickingMode = PickingMode.Ignore;
            this.Insert(0, this.gridBackground);
            this.gridBackground.StretchToParentSize();
            this.gridBackground.SendToBack();

            this.connectionLayer = new BehaviorTreeConnectionLayer(this);
            this.connectionLayer.StretchToParentSize();
            this.contentViewContainer.Add(this.connectionLayer);
            this.connectionLayer.SendToBack();
            this.connectionLayer.SetConnectionStyle(this.connectionStyle);

            this.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            this.searchWindowProvider = ScriptableObject.CreateInstance<BehaviorTreeSearchWindowProvider>();
            this.searchWindowProvider.Initialize(window, this);
            this.nodeCreationRequest = this.OpenSearchWindow;

            this.graphViewChanged += this.OnGraphViewChanged;
            this.viewTransformChanged += _ => this.gridBackground.MarkDirtyRepaint();
            this.viewTransformChanged += _ => this.connectionLayer.MarkDirtyRepaint();
            this.viewTransformChanged += _ => this.miniMap?.MarkDirtyRepaint();
            this.RegisterCallback<KeyDownEvent>(this.OnKeyDownEvent, TrickleDown.TrickleDown);
            this.RegisterCallback<MouseUpEvent>(this.OnMouseUpEvent, TrickleDown.TrickleDown);
        }

        public BehaviorTreeAsset Asset { get; private set; }

        public BehaviorTreeEditorWindow GetWindow()
        {
            return this.window;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> ports = new();
            foreach (Port port in this.ports.ToList())
            {
                if (port == startPort)
                {
                    continue;
                }

                if (port.node == startPort.node)
                {
                    continue;
                }

                if (port.direction == startPort.direction)
                {
                    continue;
                }

                ports.Add(port);
            }

            return ports;
        }

        public void PopulateView(BehaviorTreeAsset asset)
        {
            this.isPopulating = true;
            this.Asset = asset;
            this.DeleteElements(this.graphElements.ToList());
            this.nodeViews.Clear();

            if (asset == null)
            {
                this.EnsureMiniMap();
                this.isPopulating = false;
                return;
            }

            asset.EnsureInitialized();

            foreach (BehaviorTreeEditorNodeData node in asset.Nodes)
            {
                this.AddNodeView(node);
            }

            foreach (BehaviorTreeEditorNodeData node in asset.Nodes)
            {
                if (!this.nodeViews.TryGetValue(node.NodeId, out BehaviorTreeNodeView parentView) || parentView.OutputPort == null)
                {
                    continue;
                }

                foreach (string childId in node.ChildIds)
                {
                    if (!this.nodeViews.TryGetValue(childId, out BehaviorTreeNodeView childView) || childView.InputPort == null)
                    {
                        continue;
                    }

                    Edge edge = parentView.OutputPort.ConnectTo(childView.InputPort);
                    this.AddElement(edge);
                    this.ConfigureEdgeVisual(edge);
                }
            }

            this.UpdateViewTransform(asset.ViewPosition, asset.ViewScale == Vector3.zero ? Vector3.one : asset.ViewScale);
            this.EnsureMiniMap();
            this.RefreshDebugStates(this.window.GetActiveSnapshot());
            this.connectionLayer.MarkDirtyRepaint();
            this.miniMap?.MarkDirtyRepaint();
            this.isPopulating = false;
        }

        public void CreateNode(BehaviorTreeNodeKind nodeKind, Vector2 localMousePosition)
        {
            if (this.Asset == null)
            {
                return;
            }

            Vector2 contentPosition = this.contentViewContainer.WorldToLocal(this.LocalToWorld(localMousePosition));
            this.CreateNodeAtContentPosition(nodeKind, contentPosition);
        }

        public void CreateNodeAtContentPosition(BehaviorTreeNodeKind nodeKind, Vector2 contentPosition)
        {
            if (this.Asset == null)
            {
                return;
            }

            Undo.RecordObject(this.Asset, "Create Behavior Tree Node");
            BehaviorTreeEditorNodeData node = this.Asset.AddNode(nodeKind, contentPosition);
            this.AddNodeView(node);
            EditorUtility.SetDirty(this.Asset);
            this.window.SelectNode(this.nodeViews[node.NodeId]);
        }

        public Vector2 GetPendingNodeCreationContentPosition()
        {
            if (!this.hasPendingNodeCreationPosition)
            {
                return this.contentViewContainer.WorldToLocal(this.layout.center);
            }

            return this.pendingNodeCreationContentPosition;
        }

        public void PasteNodes(Vector2? centerPosition = null)
        {
            if (this.Asset == null || clipboard == null || clipboard.Nodes.Count == 0)
            {
                return;
            }

            Undo.RecordObject(this.Asset, "Paste Behavior Tree Nodes");

            Vector2 minPosition = new(float.MaxValue, float.MaxValue);
            foreach (BehaviorTreeEditorNodeData clipboardNode in clipboard.Nodes)
            {
                minPosition.x = Mathf.Min(minPosition.x, clipboardNode.Position.xMin);
                minPosition.y = Mathf.Min(minPosition.y, clipboardNode.Position.yMin);
            }

            Vector2 pasteOrigin = centerPosition ?? this.contentViewContainer.WorldToLocal(this.layout.center);
            Vector2 offset = pasteOrigin - minPosition + new Vector2(40, 40);

            Dictionary<string, BehaviorTreeEditorNodeData> pastedNodes = new();
            foreach (BehaviorTreeEditorNodeData clipboardNode in clipboard.Nodes)
            {
                BehaviorTreeEditorNodeData pastedNode = clipboardNode.Clone();
                pastedNode.NodeId = Guid.NewGuid().ToString("N");
                pastedNode.ChildIds.Clear();
                pastedNode.Position = new Rect(
                    clipboardNode.Position.x + offset.x,
                    clipboardNode.Position.y + offset.y,
                    clipboardNode.Position.width,
                    clipboardNode.Position.height);

                this.Asset.Nodes.Add(pastedNode);
                pastedNodes.Add(clipboardNode.NodeId, pastedNode);
                this.AddNodeView(pastedNode);
            }

            foreach ((string parentId, string childId) in clipboard.Connections)
            {
                if (!pastedNodes.TryGetValue(parentId, out BehaviorTreeEditorNodeData parentNode) || !pastedNodes.TryGetValue(childId, out BehaviorTreeEditorNodeData childNode))
                {
                    continue;
                }

                parentNode.ChildIds.Add(childNode.NodeId);
            }

            EditorUtility.SetDirty(this.Asset);
            this.PopulateView(this.Asset);

            this.ClearSelection();
            foreach (BehaviorTreeEditorNodeData pastedNode in pastedNodes.Values)
            {
                if (this.nodeViews.TryGetValue(pastedNode.NodeId, out BehaviorTreeNodeView pastedView))
                {
                    this.AddToSelection(pastedView);
                }
            }

            if (pastedNodes.Values.FirstOrDefault() is BehaviorTreeEditorNodeData firstPastedNode && this.nodeViews.TryGetValue(firstPastedNode.NodeId, out BehaviorTreeNodeView firstPastedView))
            {
                this.window.SelectNode(firstPastedView);
            }
        }

        public void RefreshNodeViews()
        {
            foreach (BehaviorTreeNodeView nodeView in this.nodeViews.Values)
            {
                nodeView.RefreshView(BehaviorTreeNodeState.Inactive);
            }

            this.RefreshDebugStates(this.window.GetActiveSnapshot());
        }

        public void FrameAllNodes()
        {
            this.FrameAll();
        }

        public void AutoLayoutTree()
        {
            if (this.Asset == null)
            {
                return;
            }

            Undo.RecordObject(this.Asset, "Auto Layout Behavior Tree");

            const float horizontalSpacing = 60f;
            const float verticalSpacing = 150f;
            const float startX = 120f;
            const float startY = 100f;

            Dictionary<string, float> subtreeWidths = new();
            float Measure(string nodeId)
            {
                if (string.IsNullOrWhiteSpace(nodeId))
                {
                    return 240f;
                }

                if (subtreeWidths.TryGetValue(nodeId, out float cachedWidth))
                {
                    return cachedWidth;
                }

                BehaviorTreeEditorNodeData node = this.Asset.GetNode(nodeId);
                if (node == null || node.ChildIds.Count == 0)
                {
                    subtreeWidths[nodeId] = 240f;
                    return 240f;
                }

                float totalWidth = 0f;
                for (int index = 0; index < node.ChildIds.Count; ++index)
                {
                    totalWidth += Measure(node.ChildIds[index]);
                    if (index < node.ChildIds.Count - 1)
                    {
                        totalWidth += horizontalSpacing;
                    }
                }

                totalWidth = Mathf.Max(240f, totalWidth);
                subtreeWidths[nodeId] = totalWidth;
                return totalWidth;
            }

            void Layout(string nodeId, int depth, float left)
            {
                BehaviorTreeEditorNodeData node = this.Asset.GetNode(nodeId);
                if (node == null)
                {
                    return;
                }

                float width = Measure(nodeId);
                node.Position = new Rect(left + (width - node.Position.width) * 0.5f, startY + depth * verticalSpacing, node.Position.width, node.Position.height);

                float childLeft = left;
                foreach (string childId in node.ChildIds)
                {
                    float childWidth = Measure(childId);
                    Layout(childId, depth + 1, childLeft);
                    childLeft += childWidth + horizontalSpacing;
                }
            }

            string rootNodeId = this.Asset.RootNodeId;
            if (!string.IsNullOrWhiteSpace(rootNodeId))
            {
                Layout(rootNodeId, 0, startX);
            }

            HashSet<string> connectedNodeIds = new(this.Asset.Nodes.SelectMany(node => node.ChildIds))
            {
                this.Asset.RootNodeId
            };

            float orphanX = startX;
            float orphanY = startY + 5 * verticalSpacing;
            foreach (BehaviorTreeEditorNodeData node in this.Asset.Nodes)
            {
                if (connectedNodeIds.Contains(node.NodeId) || node.NodeKind == BehaviorTreeNodeKind.Root)
                {
                    continue;
                }

                node.Position = new Rect(orphanX, orphanY, node.Position.width, node.Position.height);
                orphanX += node.Position.width + horizontalSpacing;
            }

            EditorUtility.SetDirty(this.Asset);
            this.PopulateView(this.Asset);
            this.FrameAllNodes();
        }

        public void AlignSelectionLeft()
        {
            List<BehaviorTreeNodeView> selectedNodeViews = this.GetSelectedNodeViews();
            if (selectedNodeViews.Count < 2)
            {
                return;
            }

            float left = selectedNodeViews.Min(view => view.GetPosition().xMin);
            this.ApplyNodePositions(selectedNodeViews, view =>
            {
                Rect position = view.GetPosition();
                position.x = left;
                return position;
            }, "Align Behavior Tree Nodes Left");
        }

        public void AlignSelectionRight()
        {
            List<BehaviorTreeNodeView> selectedNodeViews = this.GetSelectedNodeViews();
            if (selectedNodeViews.Count < 2)
            {
                return;
            }

            float right = selectedNodeViews.Max(view => view.GetPosition().xMax);
            this.ApplyNodePositions(selectedNodeViews, view =>
            {
                Rect position = view.GetPosition();
                position.x = right - position.width;
                return position;
            }, "Align Behavior Tree Nodes Right");
        }

        public void AlignSelectionTop()
        {
            List<BehaviorTreeNodeView> selectedNodeViews = this.GetSelectedNodeViews();
            if (selectedNodeViews.Count < 2)
            {
                return;
            }

            float top = selectedNodeViews.Min(view => view.GetPosition().yMin);
            this.ApplyNodePositions(selectedNodeViews, view =>
            {
                Rect position = view.GetPosition();
                position.y = top;
                return position;
            }, "Align Behavior Tree Nodes Top");
        }

        public void AlignSelectionBottom()
        {
            List<BehaviorTreeNodeView> selectedNodeViews = this.GetSelectedNodeViews();
            if (selectedNodeViews.Count < 2)
            {
                return;
            }

            float bottom = selectedNodeViews.Max(view => view.GetPosition().yMax);
            this.ApplyNodePositions(selectedNodeViews, view =>
            {
                Rect position = view.GetPosition();
                position.y = bottom - position.height;
                return position;
            }, "Align Behavior Tree Nodes Bottom");
        }

        public void AlignSelectionHorizontalCenter()
        {
            List<BehaviorTreeNodeView> selectedNodeViews = this.GetSelectedNodeViews();
            if (selectedNodeViews.Count < 2)
            {
                return;
            }

            float center = selectedNodeViews.Average(view => view.GetPosition().center.x);
            this.ApplyNodePositions(selectedNodeViews, view =>
            {
                Rect position = view.GetPosition();
                position.x = center - position.width * 0.5f;
                return position;
            }, "Align Behavior Tree Nodes Horizontal Center");
        }

        public void AlignSelectionVerticalCenter()
        {
            List<BehaviorTreeNodeView> selectedNodeViews = this.GetSelectedNodeViews();
            if (selectedNodeViews.Count < 2)
            {
                return;
            }

            float center = selectedNodeViews.Average(view => view.GetPosition().center.y);
            this.ApplyNodePositions(selectedNodeViews, view =>
            {
                Rect position = view.GetPosition();
                position.y = center - position.height * 0.5f;
                return position;
            }, "Align Behavior Tree Nodes Vertical Center");
        }

        public void DistributeSelectionHorizontal()
        {
            List<BehaviorTreeNodeView> selectedNodeViews = this.GetSelectedNodeViews().OrderBy(view => view.GetPosition().center.x).ToList();
            if (selectedNodeViews.Count < 3)
            {
                return;
            }

            float left = selectedNodeViews.First().GetPosition().xMin;
            float right = selectedNodeViews.Last().GetPosition().xMax;
            float contentWidth = selectedNodeViews.Sum(view => view.GetPosition().width);
            float spacing = (right - left - contentWidth) / (selectedNodeViews.Count - 1);
            float currentLeft = left;

            this.ApplyNodePositions(selectedNodeViews, view =>
            {
                Rect position = view.GetPosition();
                position.x = currentLeft;
                currentLeft += position.width + spacing;
                return position;
            }, "Distribute Behavior Tree Nodes Horizontal");
        }

        public void DistributeSelectionVertical()
        {
            List<BehaviorTreeNodeView> selectedNodeViews = this.GetSelectedNodeViews().OrderBy(view => view.GetPosition().center.y).ToList();
            if (selectedNodeViews.Count < 3)
            {
                return;
            }

            float top = selectedNodeViews.First().GetPosition().yMin;
            float bottom = selectedNodeViews.Last().GetPosition().yMax;
            float contentHeight = selectedNodeViews.Sum(view => view.GetPosition().height);
            float spacing = (bottom - top - contentHeight) / (selectedNodeViews.Count - 1);
            float currentTop = top;

            this.ApplyNodePositions(selectedNodeViews, view =>
            {
                Rect position = view.GetPosition();
                position.y = currentTop;
                currentTop += position.height + spacing;
                return position;
            }, "Distribute Behavior Tree Nodes Vertical");
        }

        public void SetMiniMapVisible(bool visible)
        {
            this.EnsureMiniMap();
            if (this.miniMap == null)
            {
                return;
            }

            this.miniMap.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void SetMiniMapRect(Rect rect)
        {
            this.EnsureMiniMap();
            this.miniMap?.SetRect(rect);
        }

        public void SetGridVisible(bool visible)
        {
            if (this.gridBackground == null)
            {
                return;
            }

            this.gridBackground.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void SetConnectionStyle(BehaviorTreeConnectionStyle style)
        {
            this.connectionStyle = style;
            this.connectionLayer?.SetConnectionStyle(style);
            this.connectionLayer?.MarkDirtyRepaint();
            this.miniMap?.MarkDirtyRepaint();
        }

        public BehaviorTreeConnectionStyle GetConnectionStyle()
        {
            return this.connectionStyle;
        }

        public void SelectNode(string nodeId)
        {
            if (string.IsNullOrWhiteSpace(nodeId))
            {
                return;
            }

            if (!this.nodeViews.TryGetValue(nodeId, out BehaviorTreeNodeView nodeView))
            {
                return;
            }

            this.ClearSelection();
            this.AddToSelection(nodeView);
            this.window.SelectNode(nodeView);
            this.FrameSelection();
        }

        public void RefreshDebugStates(BehaviorTreeDebugSnapshot snapshot)
        {
            foreach (BehaviorTreeNodeView nodeView in this.nodeViews.Values)
            {
                BehaviorTreeNodeState state = BehaviorTreeNodeState.Inactive;
                if (snapshot != null && snapshot.NodeStates.TryGetValue(nodeView.Data.NodeId, out BehaviorTreeNodeState debugState))
                {
                    state = debugState;
                }

                nodeView.RefreshView(state);
            }
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            evt.menu.AppendSeparator();

            this.CachePendingNodeCreationPosition(evt.localMousePosition);
            evt.menu.AppendAction("Create Node...", _ => this.OpenSearchWindow(evt.localMousePosition));
            evt.menu.AppendAction("Edit/Copy", _ => this.CopySelection(), _ => this.CanCopySelection() ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            evt.menu.AppendAction("Edit/Paste", _ => this.PasteNodes(evt.localMousePosition), _ => clipboard != null && clipboard.Nodes.Count > 0 ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            evt.menu.AppendAction("Edit/Duplicate", _ => this.DuplicateSelection(evt.localMousePosition), _ => this.CanCopySelection() ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            evt.menu.AppendAction("Edit/Delete", _ => this.DeleteSelectionNodes(), _ => this.CanDeleteSelection() ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            evt.menu.AppendSeparator();
            evt.menu.AppendAction("View/Frame All", _ => this.FrameAllNodes());
            evt.menu.AppendAction("View/Auto Layout", _ => this.AutoLayoutTree(), _ => this.Asset == null ? DropdownMenuAction.Status.Disabled : DropdownMenuAction.Status.Normal);
            evt.menu.AppendSeparator();
            evt.menu.AppendAction("Layout/Align Left", _ => this.AlignSelectionLeft(), _ => this.CanAlignSelection() ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            evt.menu.AppendAction("Layout/Align Right", _ => this.AlignSelectionRight(), _ => this.CanAlignSelection() ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            evt.menu.AppendAction("Layout/Align Top", _ => this.AlignSelectionTop(), _ => this.CanAlignSelection() ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            evt.menu.AppendAction("Layout/Align Bottom", _ => this.AlignSelectionBottom(), _ => this.CanAlignSelection() ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            evt.menu.AppendAction("Layout/Align Horizontal Center", _ => this.AlignSelectionHorizontalCenter(), _ => this.CanAlignSelection() ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            evt.menu.AppendAction("Layout/Align Vertical Center", _ => this.AlignSelectionVerticalCenter(), _ => this.CanAlignSelection() ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            evt.menu.AppendAction("Layout/Distribute Horizontal", _ => this.DistributeSelectionHorizontal(), _ => this.CanDistributeSelection() ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            evt.menu.AppendAction("Layout/Distribute Vertical", _ => this.DistributeSelectionVertical(), _ => this.CanDistributeSelection() ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
        }

        private void AddNodeView(BehaviorTreeEditorNodeData node)
        {
            BehaviorTreeNodeView view = new(node, this.window.SelectNode, () =>
            {
                this.window.MarkAssetDirty();
                this.connectionLayer.MarkDirtyRepaint();
            });
            this.nodeViews.Add(node.NodeId, view);
            this.AddElement(view);
            view.RegisterCallback<GeometryChangedEvent>(_ => this.connectionLayer.MarkDirtyRepaint());
            view.RegisterCallback<MouseUpEvent>(_ => this.connectionLayer.MarkDirtyRepaint(), TrickleDown.TrickleDown);
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (this.Asset == null || this.isPopulating)
            {
                return graphViewChange;
            }

            if (graphViewChange.elementsToRemove != null)
            {
                foreach (GraphElement element in graphViewChange.elementsToRemove)
                {
                    if (element is Edge edge)
                    {
                        this.Disconnect(edge);
                    }

                    if (element is BehaviorTreeNodeView nodeView)
                    {
                        Undo.RecordObject(this.Asset, "Delete Behavior Tree Node");
                        this.Asset.RemoveNode(nodeView.Data.NodeId);
                        this.nodeViews.Remove(nodeView.Data.NodeId);
                    }
                }
            }

            if (graphViewChange.edgesToCreate != null)
            {
                foreach (Edge edge in graphViewChange.edgesToCreate)
                {
                    this.Connect(edge);
                }
            }

            this.window.MarkAssetDirty();
            this.connectionLayer.MarkDirtyRepaint();
            return graphViewChange;
        }

        private void OnKeyDownEvent(KeyDownEvent evt)
        {
            bool isControlPressed = evt.ctrlKey || evt.commandKey;
            if (isControlPressed && evt.keyCode == KeyCode.C)
            {
                this.CopySelection();
                evt.StopImmediatePropagation();
                return;
            }

            if (isControlPressed && evt.keyCode == KeyCode.V)
            {
                this.PasteNodes();
                evt.StopImmediatePropagation();
                return;
            }

            if (isControlPressed && evt.keyCode == KeyCode.D)
            {
                this.CopySelection();
                this.PasteNodes();
                evt.StopImmediatePropagation();
                return;
            }

            if (evt.keyCode == KeyCode.Delete || evt.keyCode == KeyCode.Backspace)
            {
                this.DeleteSelectionNodes();
                evt.StopImmediatePropagation();
            }
        }

        private void CopySelection()
        {
            List<BehaviorTreeNodeView> selectedNodeViews = this.selection.OfType<BehaviorTreeNodeView>().Where(view => BehaviorTreeEditorUtility.CanDelete(view.Data)).ToList();
            if (selectedNodeViews.Count == 0)
            {
                return;
            }

            clipboard = new ClipboardSelection();
            HashSet<string> selectedIds = new(selectedNodeViews.Select(view => view.Data.NodeId));
            foreach (BehaviorTreeNodeView nodeView in selectedNodeViews)
            {
                clipboard.Nodes.Add(nodeView.Data.Clone());
            }

            foreach (BehaviorTreeNodeView nodeView in selectedNodeViews)
            {
                foreach (string childId in nodeView.Data.ChildIds)
                {
                    if (selectedIds.Contains(childId))
                    {
                        clipboard.Connections.Add((nodeView.Data.NodeId, childId));
                    }
                }
            }
        }

        private void DuplicateSelection(Vector2? centerPosition = null)
        {
            if (!this.CanCopySelection())
            {
                return;
            }

            this.CopySelection();
            this.PasteNodes(centerPosition);
        }

        private void ApplyNodePositions(List<BehaviorTreeNodeView> nodeViewsToMove, Func<BehaviorTreeNodeView, Rect> positionFactory, string undoName)
        {
            if (this.Asset == null || nodeViewsToMove.Count == 0)
            {
                return;
            }

            Undo.RecordObject(this.Asset, undoName);
            foreach (BehaviorTreeNodeView nodeView in nodeViewsToMove)
            {
                nodeView.SetPosition(positionFactory(nodeView));
            }

            EditorUtility.SetDirty(this.Asset);
            this.window.MarkAssetDirty();
        }

        private List<BehaviorTreeNodeView> GetSelectedNodeViews()
        {
            return this.selection.OfType<BehaviorTreeNodeView>().ToList();
        }

        private void DeleteSelectionNodes()
        {
            List<GraphElement> elements = this.selection
                .Where(element => element is BehaviorTreeNodeView nodeView ? BehaviorTreeEditorUtility.CanDelete(nodeView.Data) : element is Edge)
                .Cast<GraphElement>()
                .ToList();

            if (elements.Count == 0)
            {
                return;
            }

            this.DeleteElements(elements);
            this.connectionLayer.MarkDirtyRepaint();
        }

        private bool CanCopySelection()
        {
            return this.selection.OfType<BehaviorTreeNodeView>().Any(view => BehaviorTreeEditorUtility.CanDelete(view.Data));
        }

        private bool CanAlignSelection()
        {
            return this.GetSelectedNodeViews().Count >= 2;
        }

        private bool CanDistributeSelection()
        {
            return this.GetSelectedNodeViews().Count >= 3;
        }

        private bool CanDeleteSelection()
        {
            return this.selection.Any(element => element is Edge || element is BehaviorTreeNodeView nodeView && BehaviorTreeEditorUtility.CanDelete(nodeView.Data));
        }

        private void OpenSearchWindow(NodeCreationContext context)
        {
            Vector2 windowMousePosition = this.window.rootVisualElement.WorldToLocal(context.screenMousePosition);
            Vector2 graphLocalPosition = this.window.rootVisualElement.ChangeCoordinatesTo(this, windowMousePosition);
            this.CachePendingNodeCreationPosition(graphLocalPosition);
            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), this.searchWindowProvider);
        }

        private void OpenSearchWindow(Vector2 localMousePosition)
        {
            this.CachePendingNodeCreationPosition(localMousePosition);
            Vector2 screenMousePosition = GUIUtility.GUIToScreenPoint(localMousePosition);
            SearchWindow.Open(new SearchWindowContext(screenMousePosition), this.searchWindowProvider);
        }

        private void OnMouseUpEvent(MouseUpEvent evt)
        {
            if (evt.button != 1)
            {
                return;
            }

            this.CachePendingNodeCreationPosition(evt.localMousePosition);
        }

        private void CachePendingNodeCreationPosition(Vector2 graphLocalPosition)
        {
            this.pendingNodeCreationContentPosition = this.contentViewContainer.WorldToLocal(this.LocalToWorld(graphLocalPosition));
            this.hasPendingNodeCreationPosition = true;
        }

        private void EnsureMiniMap()
        {
            if (this.miniMap != null && this.miniMap.parent != null)
            {
                return;
            }

            this.miniMap = new BehaviorTreeMiniMap(this);
            this.miniMap.SetRect(this.window.LoadMiniMapRect());
            this.Add(this.miniMap);
            this.miniMap.BringToFront();
        }

        private void Connect(Edge edge)
        {
            if (edge.output?.node is not BehaviorTreeNodeView parentView || edge.input?.node is not BehaviorTreeNodeView childView)
            {
                return;
            }

            Undo.RecordObject(this.Asset, "Connect Behavior Tree Nodes");
            if (!parentView.Data.ChildIds.Contains(childView.Data.NodeId))
            {
                parentView.Data.ChildIds.Add(childView.Data.NodeId);
            }

            this.ConfigureEdgeVisual(edge);
            this.connectionLayer.MarkDirtyRepaint();
            this.miniMap?.MarkDirtyRepaint();
        }

        private void Disconnect(Edge edge)
        {
            if (edge.output?.node is not BehaviorTreeNodeView parentView || edge.input?.node is not BehaviorTreeNodeView childView)
            {
                return;
            }

            Undo.RecordObject(this.Asset, "Disconnect Behavior Tree Nodes");
            parentView.Data.ChildIds.RemoveAll(nodeId => nodeId == childView.Data.NodeId);
            this.connectionLayer.MarkDirtyRepaint();
            this.miniMap?.MarkDirtyRepaint();
        }

        public IEnumerable<BehaviorTreeNodeView> GetNodeViews()
        {
            return this.nodeViews.Values;
        }

        private void ConfigureEdgeVisual(Edge edge)
        {
            edge.style.display = DisplayStyle.None;
            edge.style.opacity = 0f;
            edge.pickingMode = PickingMode.Ignore;
            if (edge.edgeControl != null)
            {
                edge.edgeControl.style.display = DisplayStyle.None;
                edge.edgeControl.style.opacity = 0f;
                edge.edgeControl.capRadius = 0f;
                edge.edgeControl.edgeWidth = 0;
                edge.edgeControl.drawFromCap = false;
                edge.edgeControl.drawToCap = false;
            }
        }
    }
}
