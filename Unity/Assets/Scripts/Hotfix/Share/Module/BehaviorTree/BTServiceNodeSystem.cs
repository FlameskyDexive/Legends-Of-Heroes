using System;

namespace ET
{
    public static partial class BTRuntimeNodeSystem
    {
        private static void OnStart(BTServiceNode self)
        {
            BTServiceNodeData definition = self.Definition as BTServiceNodeData;
            if (self.Children.Count == 0)
            {
                self.Fail();
                return;
            }

            if (definition == null)
            {
                Log.Error($"behavior tree service node definition invalid: {self.Definition?.GetType().FullName}");
                self.Fail();
                return;
            }

            self.ServiceHandler = BTServiceDispatcher.Instance.Get(definition.HandlerName);
            if (self.ServiceHandler == null)
            {
                Log.Error($"behavior tree service handler not found: {definition.HandlerName}");
                self.Fail();
                return;
            }

            RunServiceLoop(self).Coroutine();
            self.Children[0].Start();
        }

        private static void HandleChildCompleted(BTServiceNode self, BTRuntimeNode child, BTNodeState state)
        {
            if (state == BTNodeState.Success)
            {
                self.Succeed();
                return;
            }

            self.Fail();
        }

        private static async ETTask RunServiceLoop(BTServiceNode self)
        {
            ETCancellationToken token = self.CancellationToken;
            BTServiceNodeData definition = self.Definition as BTServiceNodeData;
            if (definition == null)
            {
                self.Fail();
                return;
            }

            try
            {
                while (!token.IsCancel() && !self.Runner.IsDisposed)
                {
                    await self.ServiceHandler.Tick(self.Runner.Context, self.Definition, token);
                    if (token.IsCancel() || self.Runner.IsDisposed)
                    {
                        return;
                    }

                    await WaitAsync(self.Runner, definition.IntervalMilliseconds, token);
                }
            }
            catch (Exception exception)
            {
                if (token.IsCancel())
                {
                    return;
                }

                LogException(self.Runner, exception, self.Definition);
                if (self.Children.Count > 0)
                {
                    self.Children[0].Stop();
                }

                self.Fail();
            }
        }
    }
}
