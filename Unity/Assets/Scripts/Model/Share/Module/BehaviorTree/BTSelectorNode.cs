namespace ET
{
    [EnableClass]
    public sealed class BTSelectorNode : BTRuntimeNode
    {
        public int CurrentIndex;

        public BTSelectorNode(BehaviorTreeRunner runner, BTNodeData definition, BTRuntimeNode parent) : base(runner, definition, parent)
        {
        }
    }
}

