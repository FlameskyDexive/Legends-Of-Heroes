namespace ET
{
    [EnableClass]
    public sealed class BTConditionNode : BTRuntimeNode
    {
        public BTConditionNode(BehaviorTreeRunner runner, BTNodeData definition, BTRuntimeNode parent) : base(runner, definition, parent)
        {
        }
    }
}

