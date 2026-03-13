using System;

namespace ET
{
    public static partial class BehaviorTreeRuntimeNodeSystem
    {
        private static void OnStart(BehaviorTreeServiceNode self)
        {
            if (self.Children.Count == 0)
            {
                self.Fail();
                return;
            }

            self.ServiceHandler = BehaviorTreeServiceDispatcher.Instance.Get(self.Definition.HandlerName);
            if (self.ServiceHandler == null)
            {
                Log.Error($"behavior tree service handler not found: {self.Definition.HandlerName}");
                self.Fail();
                return;
            }

            RunServiceLoop(self).Coroutine();
            self.Children[0].Start();
        }

        private static void HandleChildCompleted(BehaviorTreeServiceNode self, BehaviorTreeRuntimeNode child, BehaviorTreeNodeState state)
        {
            if (state == BehaviorTreeNodeState.Success)
            {
                self.Succeed();
                return;
            }

            self.Fail();
        }

        private static async ETTask RunServiceLoop(BehaviorTreeServiceNode self)
        {
            ETCancellationToken token = self.CancellationToken;
            try
            {
                while (!token.IsCancel() && !self.Runner.IsDisposed)
                {
                    await self.ServiceHandler.Tick(self.Runner.Context, self.Definition, token);
                    if (token.IsCancel() || self.Runner.IsDisposed)
                    {
                        return;
                    }

                    await WaitAsync(self.Runner, self.Definition.IntervalMilliseconds, token);
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
