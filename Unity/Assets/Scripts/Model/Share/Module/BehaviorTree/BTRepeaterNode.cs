namespace ET
{
    [EnableClass]
    public sealed class BTRepeaterNode : BTRuntimeNode
    {
        public int CurrentLoopCount;

        public BTRepeaterNode(BTRunner runner, BTNodeData definition, BTRuntimeNode parent) : base(runner, definition, parent)
        {
        }
    }
}

