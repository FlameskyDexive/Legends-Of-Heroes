namespace ET
{
    [EnableClass]
    public sealed class BTSequenceNode : BTRuntimeNode
    {
        public int CurrentIndex;

        public BTSequenceNode(BTRunner runner, BTNodeData definition, BTRuntimeNode parent) : base(runner, definition, parent)
        {
        }
    }
}

