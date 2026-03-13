namespace ET
{
    [EnableClass]
    public sealed class BehaviorTreeBlackboardConditionNode : BehaviorTreeRuntimeNode
    {
        public long ObserverId;

        public BehaviorTreeBlackboardConditionNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
        {
        }
    }
}

