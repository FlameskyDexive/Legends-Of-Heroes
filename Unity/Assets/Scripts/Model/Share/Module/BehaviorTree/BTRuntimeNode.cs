using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public abstract class BTRuntimeNode
    {
        protected BTRuntimeNode(BehaviorTreeRunner runner, BTNodeData definition, BTRuntimeNode parent)
        {
            this.Runner = runner;
            this.Definition = definition;
            this.Parent = parent;
            this.NodeId = definition?.NodeId ?? string.Empty;
        }

        public BehaviorTreeRunner Runner;

        public BTNodeData Definition;

        public BTRuntimeNode Parent;

        public ETCancellationToken CancellationToken;

        public string NodeId;

        public BehaviorTreeNodeState State = BehaviorTreeNodeState.Inactive;

        public List<BTRuntimeNode> Children = new();
    }
}

