namespace ET
{
    [EnableClass]
    public sealed class BTFailerNode : BTRuntimeNode
    {
        public BTFailerNode(BehaviorTreeRunner runner, BTNodeData definition, BTRuntimeNode parent) : base(runner, definition, parent)
        {
        }
    }
}

