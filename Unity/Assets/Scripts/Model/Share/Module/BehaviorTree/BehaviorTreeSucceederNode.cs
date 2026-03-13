namespace ET
{
    [EnableClass]
    public sealed class BehaviorTreeSucceederNode : BehaviorTreeRuntimeNode
    {
        public BehaviorTreeSucceederNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
        {
        }
    }
}

