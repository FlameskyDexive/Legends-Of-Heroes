using System;

namespace ET
{
    public static partial class BTRuntimeNodeSystem
    {
        private static void OnStart(BTConditionNode self)
        {
            string handlerName = self.Definition.GetHandlerName();
            ABTConditionHandler handler = BTConditionDispatcher.Instance.Get(handlerName);
            if (handler == null)
            {
                Log.Error($"behavior tree condition handler not found: {handlerName}");
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
