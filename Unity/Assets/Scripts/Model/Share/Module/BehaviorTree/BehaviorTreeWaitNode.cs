namespace ET
{
    [EnableClass]
    public sealed class BehaviorTreeWaitNode : BehaviorTreeRuntimeNode
    {
        public BehaviorTreeWaitNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
        {
        }
    }
}

