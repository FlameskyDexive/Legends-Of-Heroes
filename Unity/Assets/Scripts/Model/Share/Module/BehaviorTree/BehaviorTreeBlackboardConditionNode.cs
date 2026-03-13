using System;

namespace ET
{
    [EnableClass]
    internal sealed class BehaviorTreeBlackboardConditionNode : BehaviorTreeRuntimeNode
    {
        private IDisposable observer;

        public BehaviorTreeBlackboardConditionNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
        {
        }

        protected override void OnStart()
        {
            if (this.Children.Count == 0)
            {
                if (this.Evaluate())
                {
                    this.Succeed();
                    return;
                }

                this.Fail();
                return;
            }

            if (!this.Evaluate())
            {
                this.Fail();
                return;
            }

            this.observer = this.Runner.Blackboard.Observe(this.Definition.BlackboardKey, this.OnBlackboardChanged);
            this.Children[0].Start();
        }

        protected override void OnStop()
        {
            this.observer?.Dispose();
            this.observer = null;
            base.OnStop();
        }

        protected override void HandleChildCompleted(BehaviorTreeRuntimeNode child, BehaviorTreeNodeState state)
        {
            this.observer?.Dispose();
            this.observer = null;

            if (state == BehaviorTreeNodeState.Success)
            {
                this.Succeed();
                return;
            }

            this.Fail();
        }

        private void OnBlackboardChanged(BehaviorTreeBlackboardChange change)
        {
            if (this.State != BehaviorTreeNodeState.Running)
            {
                return;
            }

            if (this.Definition.AbortMode == BehaviorTreeAbortMode.None)
            {
                return;
            }

            if (this.Evaluate())
            {
                return;
            }

            if (this.Children.Count > 0)
            {
                this.Children[0].Stop();
            }

            this.observer?.Dispose();
            this.observer = null;
            this.Fail();
        }

        private bool Evaluate()
        {
            object currentValue = this.Runner.Blackboard.GetBoxed(this.Definition.BlackboardKey);
            return BehaviorTreeValueUtility.Compare(currentValue, this.Definition.CompareOperator, this.Definition.CompareValue);
        }
    }
}
