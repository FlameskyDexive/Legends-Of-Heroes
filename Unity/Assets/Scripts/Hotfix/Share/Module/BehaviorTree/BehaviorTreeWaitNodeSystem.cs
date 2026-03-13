using System;

namespace ET
{
    public static partial class BehaviorTreeRuntimeNodeSystem
    {
        private static void OnStart(BehaviorTreeWaitNode self)
        {
            RunWaitAsync(self).Coroutine();
        }

        private static async ETTask RunWaitAsync(BehaviorTreeWaitNode self)
        {
            ETCancellationToken token = self.CancellationToken;
            try
            {
                await WaitAsync(self.Runner, self.Definition.WaitMilliseconds, token);
                if (token.IsCancel() || self.Runner.IsDisposed)
                {
                    return;
                }

                self.Succeed();
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
