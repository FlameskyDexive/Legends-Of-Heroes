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
        private readonly Dictionary<string, BehaviorTreeNodeView> nodeViews = new();
        private readonly BehaviorTreeEditorWindow window;
        private bool isPopulating;

        public BehaviorTreeGraphView(BehaviorTreeEditorWindow window)
        {
            this.window = window;
            this.style.flexGrow = 1;

            GridBackground gridBackground = new();
            this.Insert(0, gridBackground);
            gridBackground.StretchToParentSize();

            this.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            this.graphViewChanged += this.OnGraphViewChanged;
        }

        public BehaviorTreeAsset Asset { get; private set; }

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
                }
            }

            this.UpdateViewTransform(asset.ViewPosition, asset.ViewScale == Vector3.zero ? Vector3.one : asset.ViewScale);
            this.RefreshDebugStates(this.window.GetActiveSnapshot());
            this.isPopulating = false;
        }

        public void CreateNode(BehaviorTreeNodeKind nodeKind, Vector2 localMousePosition)
        {
            if (this.Asset == null)
            {
                return;
            }

            Undo.RecordObject(this.Asset, "Create Behavior Tree Node");
            Vector2 graphPosition = this.contentViewContainer.WorldToLocal(this.LocalToWorld(localMousePosition));
            BehaviorTreeEditorNodeData node = this.Asset.AddNode(nodeKind, graphPosition);
            this.AddNodeView(node);
            EditorUtility.SetDirty(this.Asset);
            this.window.SelectNode(this.nodeViews[node.NodeId]);
        }

        public void RefreshNodeViews()
        {
            foreach (BehaviorTreeNodeView nodeView in this.nodeViews.Values)
            {
                nodeView.RefreshView(BehaviorTreeNodeState.Inactive);
            }

            this.RefreshDebugStates(this.window.GetActiveSnapshot());
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

            this.AppendCreateAction(evt, BehaviorTreeNodeKind.Sequence);
            this.AppendCreateAction(evt, BehaviorTreeNodeKind.Selector);
            this.AppendCreateAction(evt, BehaviorTreeNodeKind.Parallel);
            this.AppendCreateAction(evt, BehaviorTreeNodeKind.Inverter);
            this.AppendCreateAction(evt, BehaviorTreeNodeKind.Succeeder);
            this.AppendCreateAction(evt, BehaviorTreeNodeKind.Failer);
            this.AppendCreateAction(evt, BehaviorTreeNodeKind.Repeater);
            this.AppendCreateAction(evt, BehaviorTreeNodeKind.BlackboardCondition);
            this.AppendCreateAction(evt, BehaviorTreeNodeKind.Service);
            this.AppendCreateAction(evt, BehaviorTreeNodeKind.Action);
            this.AppendCreateAction(evt, BehaviorTreeNodeKind.Condition);
            this.AppendCreateAction(evt, BehaviorTreeNodeKind.Wait);
            this.AppendCreateAction(evt, BehaviorTreeNodeKind.SubTree);
        }

        private void AddNodeView(BehaviorTreeEditorNodeData node)
        {
            BehaviorTreeNodeView view = new(node, this.window.SelectNode, this.window.MarkAssetDirty);
            this.nodeViews.Add(node.NodeId, view);
            this.AddElement(view);
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
            return graphViewChange;
        }

        private void AppendCreateAction(ContextualMenuPopulateEvent evt, BehaviorTreeNodeKind nodeKind)
        {
            evt.menu.AppendAction($"Create/{BehaviorTreeEditorUtility.GetDefaultTitle(nodeKind)}", _ => this.CreateNode(nodeKind, evt.localMousePosition));
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
        }

        private void Disconnect(Edge edge)
        {
            if (edge.output?.node is not BehaviorTreeNodeView parentView || edge.input?.node is not BehaviorTreeNodeView childView)
            {
                return;
            }

            Undo.RecordObject(this.Asset, "Disconnect Behavior Tree Nodes");
            parentView.Data.ChildIds.RemoveAll(nodeId => nodeId == childView.Data.NodeId);
        }
    }
}
