using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ET
{
    public sealed class BTNodeView : Node
    {
        private readonly Label summaryLabel;
        private readonly Action onChanged;
        private readonly Action<BTNodeView> onSelected;
        private readonly Action<BTNodeView> onDoubleClicked;
        private readonly VisualElement topPortContainer;
        private readonly VisualElement bottomPortContainer;

        public BTNodeView(BTEditorNodeData data, Action<BTNodeView> onSelected, Action onChanged,
            Action<BTNodeView> onDoubleClicked = null)
        {
            this.Data = data;
            this.onSelected = onSelected;
            this.onChanged = onChanged;
            this.onDoubleClicked = onDoubleClicked;
            this.viewDataKey = data.NodeId;

            this.style.overflow = Overflow.Visible;

            this.inputContainer.style.display = DisplayStyle.None;
            this.outputContainer.style.display = DisplayStyle.None;

            this.topPortContainer = CreatePortContainer(true);
            this.bottomPortContainer = CreatePortContainer(false);
            this.Add(this.topPortContainer);
            this.Add(this.bottomPortContainer);

            if (BTEditorUtility.HasInputPort(data.NodeKind))
            {
                this.InputPort = this.InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                this.InputPort.portName = string.Empty;
                ConfigurePort(this.InputPort);
                this.topPortContainer.Add(this.InputPort);
            }

            if (BTEditorUtility.HasOutputPort(data.NodeKind))
            {
                this.OutputPort = this.InstantiatePort(Orientation.Vertical, Direction.Output, BTEditorUtility.GetOutputCapacity(data.NodeKind), typeof(bool));
                this.OutputPort.portName = string.Empty;
                ConfigurePort(this.OutputPort);
                this.bottomPortContainer.Add(this.OutputPort);
            }

            this.summaryLabel = new Label();
            this.summaryLabel.style.whiteSpace = WhiteSpace.Normal;
            this.summaryLabel.style.unityTextAlign = TextAnchor.UpperLeft;
            this.extensionContainer.Add(this.summaryLabel);
            this.mainContainer.style.marginTop = 6;
            this.mainContainer.style.marginBottom = 6;

            if (!BTEditorUtility.CanDelete(data))
            {
                this.capabilities &= ~Capabilities.Deletable;
            }

            this.RefreshView(BTNodeState.Inactive);
            this.SetPosition(data.Position);
            this.RegisterCallback<MouseDownEvent>(this.OnMouseDownEvent, TrickleDown.TrickleDown);
        }

        public BTEditorNodeData Data { get; }

        public Port InputPort { get; }

        public Port OutputPort { get; }

        public Vector2 GetInputAnchorWorldPosition()
        {
            Rect worldRect = this.worldBound;
            return new Vector2(worldRect.center.x, worldRect.yMin);
        }

        public Vector2 GetInputAnchorContentPosition()
        {
            Rect rect = this.GetPosition();
            return new Vector2(rect.center.x, rect.yMin);
        }

        public Vector2 GetOutputAnchorWorldPosition()
        {
            Rect worldRect = this.worldBound;
            return new Vector2(worldRect.center.x, worldRect.yMax);
        }

        public Vector2 GetOutputAnchorContentPosition()
        {
            Rect rect = this.GetPosition();
            return new Vector2(rect.center.x, rect.yMax);
        }

        public Rect GetNodeWorldRect()
        {
            return this.worldBound;
        }

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

        private void OnMouseDownEvent(MouseDownEvent evt)
        {
            if (evt.button != 0 || evt.clickCount != 2)
            {
                return;
            }

            this.onDoubleClicked?.Invoke(this);
            evt.StopPropagation();
        }

        public void RefreshView(BTNodeState debugState)
        {
            this.title = BTEditorUtility.GetNodeTitle(this.Data);
            this.summaryLabel.text = BTEditorUtility.GetNodeSummary(this.Data);
            this.titleContainer.style.backgroundColor = BTEditorUtility.GetNodeHeaderColor(this.Data.NodeKind, debugState);
            this.RefreshExpandedState();
            this.RefreshPorts();
        }

        private static VisualElement CreatePortContainer(bool isTop)
        {
            VisualElement container = new();
            container.style.position = Position.Absolute;
            container.style.left = 0;
            container.style.right = 0;
            container.style.height = 18;
            container.style.justifyContent = Justify.Center;
            container.style.alignItems = Align.Center;
            container.style.flexDirection = FlexDirection.Row;
            container.style.overflow = Overflow.Visible;
            if (isTop)
            {
                container.style.top = -9;
            }
            else
            {
                container.style.bottom = -9;
            }

            return container;
        }

        private static void ConfigurePort(Port port)
        {
            port.style.alignSelf = Align.Center;
            port.style.justifyContent = Justify.Center;
            port.style.alignItems = Align.Center;
            port.style.width = 16;
            port.style.minWidth = 16;
            port.style.maxWidth = 16;
            port.style.height = 16;
            port.style.minHeight = 16;
            port.style.maxHeight = 16;
            port.style.marginLeft = 0;
            port.style.marginRight = 0;
            port.style.marginTop = 0;
            port.style.marginBottom = 0;
            port.style.paddingLeft = 0;
            port.style.paddingRight = 0;
            port.style.paddingTop = 0;
            port.style.paddingBottom = 0;
            port.style.position = Position.Relative;
            port.style.backgroundColor = new Color(0f, 0f, 0f, 0f);
            port.style.borderBottomWidth = 0;
            port.style.borderTopWidth = 0;
            port.style.borderLeftWidth = 0;
            port.style.borderRightWidth = 0;

            foreach (VisualElement child in port.Children())
            {
                if (child is Label)
                {
                    child.style.display = DisplayStyle.None;
                    continue;
                }

                if (child.ClassListContains("connector"))
                {
                    child.style.width = 10;
                    child.style.height = 10;
                    child.style.minWidth = 10;
                    child.style.minHeight = 10;
                    child.style.maxWidth = 10;
                    child.style.maxHeight = 10;
                    child.style.marginLeft = 0;
                    child.style.marginRight = 0;
                    child.style.marginTop = 0;
                    child.style.marginBottom = 0;
                }
            }
        }
    }
}
