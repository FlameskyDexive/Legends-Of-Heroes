namespace ET
{
    public static partial class BehaviorTreeRuntimeNodeSystem
    {
        private static void OnStart(BehaviorTreeBlackboardConditionNode self)
        {
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

            self.ObserverId = self.Runner.Blackboard.Observe(self.Definition.BlackboardKey, change => OnBlackboardChanged(self, change));
            self.Children[0].Start();
        }

        private static void HandleChildCompleted(BehaviorTreeBlackboardConditionNode self, BehaviorTreeRuntimeNode child, BehaviorTreeNodeState state)
        {
            self.Runner.Blackboard.RemoveObserver(self.ObserverId);
            self.ObserverId = 0;
            if (state == BehaviorTreeNodeState.Success)
            {
                self.Succeed();
                return;
            }

            self.Fail();
        }

        private static void OnStop(BehaviorTreeBlackboardConditionNode self)
        {
            self.Runner.Blackboard.RemoveObserver(self.ObserverId);
            self.ObserverId = 0;
            self.StopChildren();
        }

        private static bool Evaluate(BehaviorTreeBlackboardConditionNode self)
        {
            object currentValue = self.Runner.Blackboard.GetBoxed(self.Definition.BlackboardKey);
            return BehaviorTreeValueUtility.Compare(currentValue, self.Definition.CompareOperator, self.Definition.CompareValue);
        }

        private static void OnBlackboardChanged(BehaviorTreeBlackboardConditionNode self, BehaviorTreeBlackboardChange change)
        {
            if (self.State != BehaviorTreeNodeState.Running)
            {
                return;
            }

            if (self.Definition.AbortMode == BehaviorTreeAbortMode.None)
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
