using System;

namespace ET
{
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
}
