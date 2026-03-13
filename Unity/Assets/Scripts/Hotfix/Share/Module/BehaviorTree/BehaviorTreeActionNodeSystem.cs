using System;

namespace ET
{
    public static partial class BehaviorTreeRuntimeNodeSystem
    {
        private static void OnStart(BehaviorTreeActionNode self)
        {
            ABehaviorTreeActionHandler handler = BehaviorTreeActionDispatcher.Instance.Get(self.Definition.HandlerName);
            if (handler == null)
            {
                Log.Error($"behavior tree action handler not found: {self.Definition.HandlerName}");
                self.Fail();
                return;
            }

            RunActionAsync(self, handler).Coroutine();
        }

        private static async ETTask RunActionAsync(BehaviorTreeActionNode self, ABehaviorTreeActionHandler handler)
        {
            ETCancellationToken token = self.CancellationToken;
            try
            {
                BehaviorTreeNodeState state = await handler.Execute(self.Runner.Context, self.Definition, token);
                if (token.IsCancel() || self.Runner.IsDisposed)
                {
                    return;
                }

                if (state == BehaviorTreeNodeState.Success)
                {
                    self.Succeed();
                    return;
                }

                self.Fail();
            }
            catch (Exception exception)
            {
                if (token.IsCancel())
                {
                    return;
                }

                LogException(self.Runner, exception, self.Definition);
                self.Fail();
            }
        }
    }
}
