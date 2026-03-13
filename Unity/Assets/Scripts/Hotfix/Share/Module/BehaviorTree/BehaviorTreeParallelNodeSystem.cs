using System;

namespace ET
{
    public static partial class BehaviorTreeRuntimeNodeSystem
    {
        private static void OnStart(BehaviorTreeParallelNode self)
        {
            self.CompletedCount = 0;
            self.SuccessCount = 0;
            self.FailureCount = 0;
            if (self.Children.Count == 0)
            {
                self.Succeed();
                return;
            }

            foreach (BehaviorTreeRuntimeNode child in self.Children)
            {
                child.Start();
            }
        }

        private static void HandleChildCompleted(BehaviorTreeParallelNode self, BehaviorTreeRuntimeNode child, BehaviorTreeNodeState state)
        {
            ++self.CompletedCount;
            if (state == BehaviorTreeNodeState.Success)
            {
                ++self.SuccessCount;
            }
            else
            {
                ++self.FailureCount;
            }

            if (self.Definition.SuccessPolicy == BehaviorTreeParallelPolicy.RequireOne && state == BehaviorTreeNodeState.Success)
            {
                StopOtherChildren(self, child);
                self.Succeed();
                return;
            }

            if (self.Definition.FailurePolicy == BehaviorTreeParallelPolicy.RequireOne && state != BehaviorTreeNodeState.Success)
            {
                StopOtherChildren(self, child);
                self.Fail();
                return;
            }

            if (self.CompletedCount < self.Children.Count)
            {
                return;
            }

            bool success = self.Definition.SuccessPolicy == BehaviorTreeParallelPolicy.RequireAll ? self.SuccessCount == self.Children.Count : self.SuccessCount > 0;
            bool failure = self.Definition.FailurePolicy == BehaviorTreeParallelPolicy.RequireAll ? self.FailureCount == self.Children.Count : self.FailureCount > 0;
            if (success && !failure)
            {
                self.Succeed();
                return;
            }

            if (failure && !success)
            {
                self.Fail();
                return;
            }

            if (success)
            {
                self.Succeed();
                return;
            }

            self.Fail();
        }

        private static void StopOtherChildren(BehaviorTreeParallelNode self, BehaviorTreeRuntimeNode completedChild)
        {
            foreach (BehaviorTreeRuntimeNode child in self.Children)
            {
                if (ReferenceEquals(child, completedChild))
                {
                    continue;
                }

                child.Stop();
            }
        }
    }
}
