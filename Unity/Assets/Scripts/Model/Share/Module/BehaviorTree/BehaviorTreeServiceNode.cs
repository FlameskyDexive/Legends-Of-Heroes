using System;

namespace ET
{
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
}
