namespace ET
{
    [EnableClass]
    internal sealed class BehaviorTreeSucceederNode : BehaviorTreeRuntimeNode
    {
        public BehaviorTreeSucceederNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
        {
        }

        protected override void OnStart()
        {
            if (this.Children.Count == 0)
            {
                this.Succeed();
                return;
            }

            this.Children[0].Start();
        }

        protected override void HandleChildCompleted(BehaviorTreeRuntimeNode child, BehaviorTreeNodeState state)
        {
            this.Succeed();
        }
    }
}
