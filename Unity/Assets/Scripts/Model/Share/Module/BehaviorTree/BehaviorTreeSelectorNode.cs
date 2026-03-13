namespace ET
{
    [EnableClass]
    public sealed class BehaviorTreeSelectorNode : BehaviorTreeRuntimeNode
    {
        public int CurrentIndex;

        public BehaviorTreeSelectorNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
        {
        }
    }
}

