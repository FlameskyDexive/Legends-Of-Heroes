namespace ET
{
    [EnableClass]
    public sealed class BehaviorTreeParallelNode : BehaviorTreeRuntimeNode
    {
        public int CompletedCount;

        public int SuccessCount;

        public int FailureCount;

        public BehaviorTreeParallelNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
        {
        }
    }
}

