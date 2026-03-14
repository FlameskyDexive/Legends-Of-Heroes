namespace ET
{
    [EnableClass]
    public sealed class BTServiceNode : BTRuntimeNode
    {
        public ABTServiceHandler ServiceHandler;

        public BTServiceNode(BehaviorTreeRunner runner, BTNodeData definition, BTRuntimeNode parent) : base(runner, definition, parent)
        {
        }
    }
}

