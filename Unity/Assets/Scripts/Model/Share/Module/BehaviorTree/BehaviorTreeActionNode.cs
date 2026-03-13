using System;

namespace ET
{
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
}
