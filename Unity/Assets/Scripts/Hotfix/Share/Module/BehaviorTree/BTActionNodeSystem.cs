using System;

namespace ET
{
    public static partial class BTRuntimeNodeSystem
    {
        private static void OnStart(BTActionNode self)
        {
            string handlerName = self.Definition.GetHandlerName();
            ABTActionHandler handler = BTActionDispatcher.Instance.Get(handlerName);
            if (handler == null)
            {
                Log.Error($"behavior tree action handler not found: {handlerName}");
                self.Fail();
                return;
            }

            RunActionAsync(self, handler).Coroutine();
        }

        private static async ETTask RunActionAsync(BTActionNode self, ABTActionHandler handler)
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
