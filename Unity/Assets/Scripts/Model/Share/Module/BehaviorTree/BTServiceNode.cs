namespace ET
{
    [EnableClass]
    public sealed class BTServiceNode : BTRuntimeNode
    {
        public ABTServiceHandler ServiceHandler;

        public BTServiceNode(BTRunner runner, BTNodeData definition, BTRuntimeNode parent) : base(runner, definition, parent)
        {
        }
    }
}

