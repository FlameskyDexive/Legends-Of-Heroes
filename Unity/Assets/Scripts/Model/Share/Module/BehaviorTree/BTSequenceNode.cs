namespace ET
{
    [EnableClass]
    public sealed class BTSequenceNode : BTRuntimeNode
    {
        public int CurrentIndex;

        public BTSequenceNode(BehaviorTreeRunner runner, BTNodeData definition, BTRuntimeNode parent) : base(runner, definition, parent)
        {
        }
    }
}

