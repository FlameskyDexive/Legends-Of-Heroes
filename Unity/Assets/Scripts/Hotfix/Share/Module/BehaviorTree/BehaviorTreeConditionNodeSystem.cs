using System;

namespace ET
{
    public static partial class BehaviorTreeRuntimeNodeSystem
    {
        private static void OnStart(BehaviorTreeConditionNode self)
        {
            ABehaviorTreeConditionHandler handler = BehaviorTreeConditionDispatcher.Instance.Get(self.Definition.HandlerName);
            if (handler == null)
            {
                Log.Error($"behavior tree condition handler not found: {self.Definition.HandlerName}");
                self.Fail();
                return;
            }

            try
            {
                if (handler.Evaluate(self.Runner.Context, self.Definition))
                {
                    self.Succeed();
                    return;
                }

                self.Fail();
            }
            catch (Exception exception)
            {
                LogException(self.Runner, exception, self.Definition);
                self.Fail();
            }
        }
    }
}
