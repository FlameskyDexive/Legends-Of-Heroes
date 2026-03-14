namespace ET
{
    [EnableClass]
    public sealed class BTRootNode : BTRuntimeNode
    {
        public BTRootNode(BehaviorTreeRunner runner, BTNodeData definition, BTRuntimeNode parent) : base(runner, definition, parent)
        {
        }
    }
}

