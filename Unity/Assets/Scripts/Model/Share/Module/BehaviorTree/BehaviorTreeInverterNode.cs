namespace ET
{
    [EnableClass]
    public sealed class BehaviorTreeInverterNode : BehaviorTreeRuntimeNode
    {
        public BehaviorTreeInverterNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
        {
        }
    }
}

