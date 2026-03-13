namespace ET
{
    [EnableClass]
    internal sealed class BehaviorTreeRootNode : BehaviorTreeRuntimeNode
    {
        public BehaviorTreeRootNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
        {
        }

        protected override void OnStart()
        {
            if (this.Children.Count == 0)
            {
                this.Fail();
                return;
            }

            this.Children[0].Start();
        }

        protected override void HandleChildCompleted(BehaviorTreeRuntimeNode child, BehaviorTreeNodeState state)
        {
            if (state == BehaviorTreeNodeState.Success)
            {
                this.Succeed();
                return;
            }

            this.Fail();
        }
    }
}
