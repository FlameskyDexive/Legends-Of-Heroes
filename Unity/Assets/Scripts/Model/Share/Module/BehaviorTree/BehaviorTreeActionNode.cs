namespace ET
{
    [EnableClass]
    public sealed class BehaviorTreeActionNode : BehaviorTreeRuntimeNode
    {
        public BehaviorTreeActionNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
        {
        }
    }
}

