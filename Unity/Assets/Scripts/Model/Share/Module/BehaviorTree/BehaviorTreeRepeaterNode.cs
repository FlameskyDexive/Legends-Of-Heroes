namespace ET
{
    [EnableClass]
    public sealed class BehaviorTreeRepeaterNode : BehaviorTreeRuntimeNode
    {
        public int CurrentLoopCount;

        public BehaviorTreeRepeaterNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
        {
        }
    }
}

