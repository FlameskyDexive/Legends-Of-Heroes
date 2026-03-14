namespace ET
{
    [EnableClass]
    public sealed class BTRepeaterNode : BTRuntimeNode
    {
        public int CurrentLoopCount;

        public BTRepeaterNode(BehaviorTreeRunner runner, BTNodeData definition, BTRuntimeNode parent) : base(runner, definition, parent)
        {
        }
    }
}

