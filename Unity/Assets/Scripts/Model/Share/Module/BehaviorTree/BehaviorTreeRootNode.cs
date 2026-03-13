namespace ET
{
    [EnableClass]
    public sealed class BehaviorTreeRootNode : BehaviorTreeRuntimeNode
    {
        public BehaviorTreeRootNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
        {
        }
    }
}

