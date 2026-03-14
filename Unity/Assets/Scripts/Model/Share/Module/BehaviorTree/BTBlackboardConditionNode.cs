namespace ET
{
    [EnableClass]
    public sealed class BTBlackboardConditionNode : BTRuntimeNode
    {
        public long ObserverId;

        public BTBlackboardConditionNode(BTRunner runner, BTNodeData definition, BTRuntimeNode parent) : base(runner, definition, parent)
        {
        }
    }
}

