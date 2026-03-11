using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ET
{
    public sealed class BehaviorTreeNodeView : Node
    {
        private readonly Label summaryLabel;
        private readonly Action onChanged;
        private readonly Action<BehaviorTreeNodeView> onSelected;

        public BehaviorTreeNodeView(BehaviorTreeEditorNodeData data, Action<BehaviorTreeNodeView> onSelected, Action onChanged)
        {
            this.Data = data;
            this.onSelected = onSelected;
            this.onChanged = onChanged;
            this.viewDataKey = data.NodeId;

            if (BehaviorTreeEditorUtility.HasInputPort(data.NodeKind))
            {
                this.InputPort = this.InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                this.InputPort.portName = string.Empty;
                this.inputContainer.Add(this.InputPort);
            }

            if (BehaviorTreeEditorUtility.HasOutputPort(data.NodeKind))
            {
                this.OutputPort = this.InstantiatePort(Orientation.Vertical, Direction.Output, BehaviorTreeEditorUtility.GetOutputCapacity(data.NodeKind), typeof(bool));
                this.OutputPort.portName = string.Empty;
                this.outputContainer.Add(this.OutputPort);
            }

            this.summaryLabel = new Label();
            this.summaryLabel.style.whiteSpace = WhiteSpace.Normal;
            this.summaryLabel.style.unityTextAlign = TextAnchor.UpperLeft;
            this.extensionContainer.Add(this.summaryLabel);

            if (!BehaviorTreeEditorUtility.CanDelete(data))
            {
                this.capabilities &= ~Capabilities.Deletable;
            }

            this.RefreshView(BehaviorTreeNodeState.Inactive);
            this.SetPosition(data.Position);
        }

        public BehaviorTreeEditorNodeData Data { get; }

        public Port InputPort { get; }

        public Port OutputPort { get; }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            this.Data.Position = newPos;
            this.onChanged?.Invoke();
        }

        public override void OnSelected()
        {
            base.OnSelected();
            this.onSelected?.Invoke(this);
        }

        public void RefreshView(BehaviorTreeNodeState debugState)
        {
            this.title = string.IsNullOrWhiteSpace(this.Data.Title)
                    ? BehaviorTreeEditorUtility.GetDefaultTitle(this.Data.NodeKind)
                    : this.Data.Title;

            this.summaryLabel.text = BuildSummary(this.Data);
            this.titleContainer.style.backgroundColor = BehaviorTreeEditorUtility.GetNodeColor(debugState);
            this.RefreshExpandedState();
            this.RefreshPorts();
        }

        private static string BuildSummary(BehaviorTreeEditorNodeData node)
        {
            return node.NodeKind switch
            {
                BehaviorTreeNodeKind.Action => $"Handler: {node.HandlerName}",
                BehaviorTreeNodeKind.Condition => $"Handler: {node.HandlerName}",
                BehaviorTreeNodeKind.Service => $"Service: {node.HandlerName}\nInterval: {node.IntervalMilliseconds}ms",
                BehaviorTreeNodeKind.Wait => $"Delay: {node.WaitMilliseconds}ms",
                BehaviorTreeNodeKind.Repeater => $"Loop: {(node.MaxLoopCount <= 0 ? "∞" : node.MaxLoopCount.ToString())}",
                BehaviorTreeNodeKind.BlackboardCondition => $"Key: {node.BlackboardKey}\nOp: {node.CompareOperator}",
                BehaviorTreeNodeKind.SubTree => $"SubTree: {node.SubTreeName}",
                BehaviorTreeNodeKind.Parallel => $"Success: {node.SuccessPolicy}\nFailure: {node.FailurePolicy}",
                _ => node.Comment,
            };
        }
    }
}
