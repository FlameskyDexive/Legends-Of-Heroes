namespace ET
{
    [EnableClass]
    public sealed class BehaviorTreeServiceNode : BehaviorTreeRuntimeNode
    {
        public ABehaviorTreeServiceHandler ServiceHandler;

        public BehaviorTreeServiceNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
        {
        }
    }
}

