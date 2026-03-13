namespace ET
{
    [EnableClass]
    public sealed class BehaviorTreeConditionNode : BehaviorTreeRuntimeNode
    {
        public BehaviorTreeConditionNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
        {
        }
    }
}

