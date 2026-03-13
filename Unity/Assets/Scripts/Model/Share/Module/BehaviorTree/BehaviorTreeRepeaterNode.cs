namespace ET
{
    [EnableClass]
    internal sealed class BehaviorTreeRepeaterNode : BehaviorTreeRuntimeNode
    {
        private int currentLoopCount;

        public BehaviorTreeRepeaterNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
        {
        }

        protected override void OnStart()
        {
            this.currentLoopCount = 0;
            if (this.Children.Count == 0)
            {
                this.Succeed();
                return;
            }

            this.Children[0].Start();
        }

        protected override void HandleChildCompleted(BehaviorTreeRuntimeNode child, BehaviorTreeNodeState state)
        {
            ++this.currentLoopCount;

            if (this.Definition.MaxLoopCount > 0 && this.currentLoopCount >= this.Definition.MaxLoopCount)
            {
                if (state == BehaviorTreeNodeState.Success)
                {
                    this.Succeed();
                    return;
                }

                this.Fail();
                return;
            }

            child.Start();
        }
    }
}
