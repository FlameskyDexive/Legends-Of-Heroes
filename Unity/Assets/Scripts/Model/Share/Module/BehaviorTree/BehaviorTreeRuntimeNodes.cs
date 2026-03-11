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

    [EnableClass]
    internal sealed class BehaviorTreeSequenceNode : BehaviorTreeRuntimeNode
    {
        private int currentIndex;

        public BehaviorTreeSequenceNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
        {
        }

        protected override void OnStart()
        {
            this.currentIndex = 0;
            this.StartNextChild();
        }

        protected override void HandleChildCompleted(BehaviorTreeRuntimeNode child, BehaviorTreeNodeState state)
        {
            if (state != BehaviorTreeNodeState.Success)
            {
                this.Fail();
                return;
            }

            ++this.currentIndex;
            this.StartNextChild();
        }

        private void StartNextChild()
        {
            if (this.currentIndex >= this.Children.Count)
            {
                this.Succeed();
                return;
            }

            this.Children[this.currentIndex].Start();
        }
    }

    [EnableClass]
    internal sealed class BehaviorTreeSelectorNode : BehaviorTreeRuntimeNode
    {
        private int currentIndex;

        public BehaviorTreeSelectorNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
        {
        }

        protected override void OnStart()
        {
            this.currentIndex = 0;
            this.StartNextChild();
        }

        protected override void HandleChildCompleted(BehaviorTreeRuntimeNode child, BehaviorTreeNodeState state)
        {
            if (state == BehaviorTreeNodeState.Success)
            {
                this.Succeed();
                return;
            }

            ++this.currentIndex;
            this.StartNextChild();
        }

        private void StartNextChild()
        {
            if (this.currentIndex >= this.Children.Count)
            {
                this.Fail();
                return;
            }

            this.Children[this.currentIndex].Start();
        }
    }

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

    [EnableClass]
    internal sealed class BehaviorTreeInverterNode : BehaviorTreeRuntimeNode
    {
        public BehaviorTreeInverterNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
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
                this.Fail();
                return;
            }

            this.Succeed();
        }
    }

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

    [EnableClass]
    internal sealed class BehaviorTreeFailerNode : BehaviorTreeRuntimeNode
    {
        public BehaviorTreeFailerNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
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
            this.Fail();
        }
    }

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

    [EnableClass]
    internal sealed class BehaviorTreeWaitNode : BehaviorTreeRuntimeNode
    {
        public BehaviorTreeWaitNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
        {
        }

        protected override void OnStart()
        {
            this.WaitAsync().Coroutine();
        }

        private async ETTask WaitAsync()
        {
            ETCancellationToken token = this.CancellationToken;
            try
            {
                await this.Runner.WaitAsync(this.Definition.WaitMilliseconds, token);
                if (token.IsCancel() || this.Runner.IsDisposed)
                {
                    return;
                }

                this.Succeed();
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
    internal sealed class BehaviorTreeSubTreeNode : BehaviorTreeRuntimeNode
    {
        private BehaviorTreeRuntimeNode subTree;

        public BehaviorTreeSubTreeNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
        {
        }

        protected override void OnStart()
        {
            this.subTree = this.Runner.CreateSubTree(this.Definition.SubTreeId, this.Definition.SubTreeName, this);
            if (this.subTree == null)
            {
                Log.Error($"behavior tree subtree not found: {this.Definition.SubTreeId}/{this.Definition.SubTreeName}");
                this.Fail();
                return;
            }

            this.subTree.Start();
        }

        protected override void OnStop()
        {
            this.subTree?.Stop();
            this.subTree = null;
            base.OnStop();
        }

        protected override void HandleChildCompleted(BehaviorTreeRuntimeNode child, BehaviorTreeNodeState state)
        {
            this.subTree = null;
            if (state == BehaviorTreeNodeState.Success)
            {
                this.Succeed();
                return;
            }

            this.Fail();
        }
    }
}
