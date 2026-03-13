using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public abstract class BehaviorTreeRuntimeNode
    {
        protected BehaviorTreeRuntimeNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent)
        {
            this.Runner = runner;
            this.Definition = definition;
            this.Parent = parent;
            this.NodeId = definition?.NodeId ?? string.Empty;
        }

        public BehaviorTreeRunner Runner;

        public BehaviorTreeNodeDefinition Definition;

        public BehaviorTreeRuntimeNode Parent;

        public ETCancellationToken CancellationToken;

        public string NodeId;

        public BehaviorTreeNodeState State = BehaviorTreeNodeState.Inactive;

        public List<BehaviorTreeRuntimeNode> Children = new();
    }
}

