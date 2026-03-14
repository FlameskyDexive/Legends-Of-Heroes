namespace ET
{
    [EnableClass]
    public sealed class BTParallelNode : BTRuntimeNode
    {
        public int CompletedCount;

        public int SuccessCount;

        public int FailureCount;

        public BTParallelNode(BTRunner runner, BTNodeData definition, BTRuntimeNode parent) : base(runner, definition, parent)
        {
        }
    }
}

