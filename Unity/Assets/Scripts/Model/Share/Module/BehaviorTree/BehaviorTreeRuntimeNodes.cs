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

    [EnableClass]
    internal sealed class BehaviorTreeServiceNode : BehaviorTreeRuntimeNode
    {
        private ABehaviorTreeServiceHandler serviceHandler;

        public BehaviorTreeServiceNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
        {
        }

        protected override void OnStart()
        {
            if (this.Children.Count == 0)
            {
                this.Fail();
                return;
            }

            this.serviceHandler = BehaviorTreeServiceDispatcher.Instance.Get(this.Definition.HandlerName);
            if (this.serviceHandler == null)
            {
                Log.Error($"behavior tree service handler not found: {this.Definition.HandlerName}");
                this.Fail();
                return;
            }

            this.RunServiceLoop().Coroutine();
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

        private async ETTask RunServiceLoop()
        {
            ETCancellationToken token = this.CancellationToken;
            try
            {
                while (!token.IsCancel() && !this.Runner.IsDisposed)
                {
                    await this.serviceHandler.Tick(this.Runner.Context, this.Definition, token);
                    if (token.IsCancel() || this.Runner.IsDisposed)
                    {
                        return;
                    }

                    await this.Runner.WaitAsync(this.Definition.IntervalMilliseconds, token);
                }
            }
            catch (Exception exception)
            {
                if (token.IsCancel())
                {
                    return;
                }

                this.Runner.LogException(exception, this.Definition);
                if (this.Children.Count > 0)
                {
                    this.Children[0].Stop();
                }

                this.Fail();
            }
        }
    }

    [EnableClass]
    internal sealed class BehaviorTreeActionNode : BehaviorTreeRuntimeNode
    {
        public BehaviorTreeActionNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
        {
        }

        protected override void OnStart()
        {
            ABehaviorTreeActionHandler handler = BehaviorTreeActionDispatcher.Instance.Get(this.Definition.HandlerName);
            if (handler == null)
            {
                Log.Error($"behavior tree action handler not found: {this.Definition.HandlerName}");
                this.Fail();
                return;
            }

            this.RunActionAsync(handler).Coroutine();
        }

        private async ETTask RunActionAsync(ABehaviorTreeActionHandler handler)
        {
            ETCancellationToken token = this.CancellationToken;
            try
            {
                BehaviorTreeNodeState state = await handler.Execute(this.Runner.Context, this.Definition, token);
                if (token.IsCancel() || this.Runner.IsDisposed)
                {
                    return;
                }

                if (state == BehaviorTreeNodeState.Success)
                {
                    this.Succeed();
                    return;
                }

                this.Fail();
            }
            catch (Exception exception)
            {
                if (token.IsCancel())
                {
                    return;
                }

                this.Runner.LogException(exception, this.Definition);
                this.Fail();
            }
        }
    }

    [EnableClass]
    internal sealed class BehaviorTreeConditionNode : BehaviorTreeRuntimeNode
    {
        public BehaviorTreeConditionNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
        {
        }

        protected override void OnStart()
        {
            ABehaviorTreeConditionHandler handler = BehaviorTreeConditionDispatcher.Instance.Get(this.Definition.HandlerName);
            if (handler == null)
            {
                Log.Error($"behavior tree condition handler not found: {this.Definition.HandlerName}");
                this.Fail();
                return;
            }

            try
            {
                if (handler.Evaluate(this.Runner.Context, this.Definition))
                {
                    this.Succeed();
                    return;
                }

                this.Fail();
            }
            catch (Exception exception)
            {
                this.Runner.LogException(exception, this.Definition);
                this.Fail();
            }
        }
    }
}
