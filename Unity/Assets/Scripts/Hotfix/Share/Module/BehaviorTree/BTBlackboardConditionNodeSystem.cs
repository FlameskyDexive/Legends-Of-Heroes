namespace ET
{
    public static partial class BTRuntimeNodeSystem
    {
        private static void OnStart(BTBlackboardConditionNode self)
        {
            BTBlackboardConditionNodeData definition = self.Definition as BTBlackboardConditionNodeData;
            if (definition == null)
            {
                self.Fail();
                return;
            }

            if (self.Children.Count == 0)
            {
                if (Evaluate(self))
                {
                    self.Succeed();
                    return;
                }

                self.Fail();
                return;
            }

            if (!Evaluate(self))
            {
                self.Fail();
                return;
            }

            self.ObserverId = self.Runner.Blackboard.Observe(definition.BlackboardKey, change => OnBlackboardChanged(self, change));
            self.Children[0].Start();
        }

        private static void HandleChildCompleted(BTBlackboardConditionNode self, BTRuntimeNode child, BTNodeState state)
        {
            self.Runner.Blackboard.RemoveObserver(self.ObserverId);
            self.ObserverId = 0;
            if (state == BTNodeState.Success)
            {
                self.Succeed();
                return;
            }

            self.Fail();
        }

        private static void OnStop(BTBlackboardConditionNode self)
        {
            self.Runner.Blackboard.RemoveObserver(self.ObserverId);
            self.ObserverId = 0;
            self.StopChildren();
        }

        private static bool Evaluate(BTBlackboardConditionNode self)
        {
            BTBlackboardConditionNodeData definition = self.Definition as BTBlackboardConditionNodeData;
            if (definition == null)
            {
                return false;
            }

            object currentValue = self.Runner.Blackboard.GetBoxed(definition.BlackboardKey);
            return BTValueUtility.Compare(currentValue, definition.CompareOperator, definition.CompareValue);
        }

        private static void OnBlackboardChanged(BTBlackboardConditionNode self, BTBlackboardChange change)
        {
            if (self.State != BTNodeState.Running)
            {
                return;
            }

            if (self.Definition is not BTBlackboardConditionNodeData definition || definition.AbortMode == BTAbortMode.None)
            {
                return;
            }

            if (Evaluate(self))
            {
                return;
            }

            if (self.Children.Count > 0)
            {
                self.Children[0].Stop();
            }

            self.Runner.Blackboard.RemoveObserver(self.ObserverId);
            self.ObserverId = 0;
            self.Fail();
        }
    }
}
