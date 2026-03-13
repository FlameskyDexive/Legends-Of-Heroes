namespace ET
{
    [EnableClass]
    public sealed class BehaviorTreeSubTreeNode : BehaviorTreeRuntimeNode
    {
        public BehaviorTreeRuntimeNode SubTree;

        public BehaviorTreeSubTreeNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
        {
        }
    }
}

