namespace ET
{
    [EnableClass]
    public sealed class BehaviorTreeSequenceNode : BehaviorTreeRuntimeNode
    {
        public int CurrentIndex;

        public BehaviorTreeSequenceNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
        {
        }
    }
}

