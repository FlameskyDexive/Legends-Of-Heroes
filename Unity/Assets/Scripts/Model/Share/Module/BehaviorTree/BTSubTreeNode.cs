namespace ET
{
    [EnableClass]
    public sealed class BTSubTreeNode : BTRuntimeNode
    {
        public BTRuntimeNode SubTree;

        public BTSubTreeNode(BehaviorTreeRunner runner, BTNodeData definition, BTRuntimeNode parent) : base(runner, definition, parent)
        {
        }
    }
}

