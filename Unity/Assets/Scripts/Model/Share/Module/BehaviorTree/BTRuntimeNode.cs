using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public abstract class BTRuntimeNode
    {
        protected BTRuntimeNode(BTRunner runner, BTNodeData definition, BTRuntimeNode parent)
        {
            this.Runner = runner;
            this.Definition = definition;
            this.Parent = parent;
            this.NodeId = definition?.NodeId ?? string.Empty;
        }

        public BTRunner Runner;

        public BTNodeData Definition;

        public BTRuntimeNode Parent;

        public ETCancellationToken CancellationToken;

        public string NodeId;

        public BTNodeState State = BTNodeState.Inactive;

        public List<BTRuntimeNode> Children = new();
    }
}

