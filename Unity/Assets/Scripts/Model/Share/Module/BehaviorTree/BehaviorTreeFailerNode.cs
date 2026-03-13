namespace ET
{
    [EnableClass]
    public sealed class BehaviorTreeFailerNode : BehaviorTreeRuntimeNode
    {
        public BehaviorTreeFailerNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
        {
        }
    }
}

