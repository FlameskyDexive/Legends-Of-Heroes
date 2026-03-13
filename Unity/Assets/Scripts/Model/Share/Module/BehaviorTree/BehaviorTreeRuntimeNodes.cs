using System;

namespace ET
{
    [EnableClass]
    internal abstract class BehaviorTreeRuntimeNode
    {
        protected BehaviorTreeRuntimeNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent)
        {
            this.Runner = runner;
            this.Definition = definition;
            this.Parent = parent;
        }

        protected BehaviorTreeRunner Runner { get; }

        protected BehaviorTreeNodeDefinition Definition { get; }

        protected BehaviorTreeRuntimeNode Parent { get; }

        protected ETCancellationToken CancellationToken { get; private set; }

        public string NodeId => this.Definition.NodeId;

        public BehaviorTreeNodeState State { get; private set; } = BehaviorTreeNodeState.Inactive;

        public System.Collections.Generic.List<BehaviorTreeRuntimeNode> Children { get; } = new();

        public void Start()
        {
            if (this.Runner.IsDisposed || this.State == BehaviorTreeNodeState.Running)
            {
                return;
            }

            this.CancellationToken = new ETCancellationToken();
            this.State = BehaviorTreeNodeState.Running;
            this.Runner.RecordState(this, this.State);
            this.OnStart();
        }

        public void Stop()
        {
            if (this.State != BehaviorTreeNodeState.Running)
            {
                return;
            }

            ETCancellationToken token = this.CancellationToken;
            this.CancellationToken = null;
            token?.Cancel();
            this.OnStop();
            this.State = BehaviorTreeNodeState.Aborted;
            this.Runner.RecordState(this, this.State);
        }

        protected bool IsCancelled => this.CancellationToken.IsCancel();

        protected void Succeed()
        {
            this.Complete(BehaviorTreeNodeState.Success);
        }

        protected void Fail()
        {
            this.Complete(BehaviorTreeNodeState.Failure);
        }

        protected virtual void OnStop()
        {
            foreach (BehaviorTreeRuntimeNode child in this.Children)
            {
                child.Stop();
            }
        }

        protected abstract void OnStart();

        protected virtual void HandleChildCompleted(BehaviorTreeRuntimeNode child, BehaviorTreeNodeState state)
        {
        }

        private void Complete(BehaviorTreeNodeState state)
        {
            if (this.State != BehaviorTreeNodeState.Running)
            {
                return;
            }

            ETCancellationToken token = this.CancellationToken;
            this.CancellationToken = null;
            token?.Cancel();
            this.State = state;
            this.Runner.RecordState(this, state);
            this.Parent?.HandleChildCompleted(this, state);
        }
    }
}
