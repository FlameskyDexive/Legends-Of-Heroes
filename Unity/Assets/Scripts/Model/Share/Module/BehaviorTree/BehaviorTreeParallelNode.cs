namespace ET
{
    [EnableClass]
    internal sealed class BehaviorTreeParallelNode : BehaviorTreeRuntimeNode
    {
        private int completedCount;
        private int successCount;
        private int failureCount;

        public BehaviorTreeParallelNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
        {
        }

        protected override void OnStart()
        {
            this.completedCount = 0;
            this.successCount = 0;
            this.failureCount = 0;

            if (this.Children.Count == 0)
            {
                this.Succeed();
                return;
            }

            foreach (BehaviorTreeRuntimeNode child in this.Children)
            {
                child.Start();
            }
        }

        protected override void HandleChildCompleted(BehaviorTreeRuntimeNode child, BehaviorTreeNodeState state)
        {
            ++this.completedCount;
            if (state == BehaviorTreeNodeState.Success)
            {
                ++this.successCount;
            }
            else
            {
                ++this.failureCount;
            }

            if (this.Definition.SuccessPolicy == BehaviorTreeParallelPolicy.RequireOne && state == BehaviorTreeNodeState.Success)
            {
                this.StopOtherChildren(child);
                this.Succeed();
                return;
            }

            if (this.Definition.FailurePolicy == BehaviorTreeParallelPolicy.RequireOne && state != BehaviorTreeNodeState.Success)
            {
                this.StopOtherChildren(child);
                this.Fail();
                return;
            }

            if (this.completedCount < this.Children.Count)
            {
                return;
            }

            bool success = this.Definition.SuccessPolicy == BehaviorTreeParallelPolicy.RequireAll
                    ? this.successCount == this.Children.Count
                    : this.successCount > 0;

            bool failure = this.Definition.FailurePolicy == BehaviorTreeParallelPolicy.RequireAll
                    ? this.failureCount == this.Children.Count
                    : this.failureCount > 0;

            if (success && !failure)
            {
                this.Succeed();
                return;
            }

            if (failure && !success)
            {
                this.Fail();
                return;
            }

            if (success)
            {
                this.Succeed();
                return;
            }

            this.Fail();
        }

        private void StopOtherChildren(BehaviorTreeRuntimeNode completedChild)
        {
            foreach (BehaviorTreeRuntimeNode child in this.Children)
            {
                if (ReferenceEquals(child, completedChild))
                {
                    continue;
                }

                child.Stop();
            }
        }
    }
}
