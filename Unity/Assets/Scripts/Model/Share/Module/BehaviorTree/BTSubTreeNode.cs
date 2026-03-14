namespace ET
{
    [EnableClass]
    public sealed class BTSubTreeNode : BTRuntimeNode
    {
        public BTRuntimeNode SubTree;

        public BTSubTreeNode(BTRunner runner, BTNodeData definition, BTRuntimeNode parent) : base(runner, definition, parent)
        {
        }
    }
}

