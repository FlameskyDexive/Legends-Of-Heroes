namespace ET
{
    [EnableClass]
    public sealed class BTActionNode : BTRuntimeNode
    {
        public BTActionNode(BehaviorTreeRunner runner, BTNodeData definition, BTRuntimeNode parent) : base(runner, definition, parent)
        {
        }
    }
}

