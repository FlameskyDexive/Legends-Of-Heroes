namespace ET
{
    [EnableClass]
    public sealed class BTRootNode : BTRuntimeNode
    {
        public BTRootNode(BTRunner runner, BTNodeData definition, BTRuntimeNode parent) : base(runner, definition, parent)
        {
        }
    }
}

