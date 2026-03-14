using System;

namespace ET
{
    public static partial class BTRuntimeNodeSystem
    {
        private static void OnStart(BTWaitNode self)
        {
            RunWaitAsync(self).Coroutine();
        }

        private static async ETTask RunWaitAsync(BTWaitNode self)
        {
            ETCancellationToken token = self.CancellationToken;
            BTWaitNodeData definition = self.Definition as BTWaitNodeData;
            if (definition == null)
            {
                self.Fail();
                return;
            }

            try
            {
                await WaitAsync(self.Runner, definition.WaitMilliseconds, token);
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
