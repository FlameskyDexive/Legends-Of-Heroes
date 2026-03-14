namespace ET
{
    [EnableClass]
    public sealed class BTWaitNode : BTRuntimeNode
    {
        public BTWaitNode(BehaviorTreeRunner runner, BTNodeData definition, BTRuntimeNode parent) : base(runner, definition, parent)
        {
        }
    }
}

