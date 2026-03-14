using System;

namespace ET
{
    public static partial class BTRuntimeNodeSystem
    {
        private static void OnStart(BTParallelNode self)
        {
            self.CompletedCount = 0;
            self.SuccessCount = 0;
            self.FailureCount = 0;
            if (self.Children.Count == 0)
            {
                self.Succeed();
                return;
            }

            foreach (BTRuntimeNode child in self.Children)
            {
                child.Start();
            }
        }

        private static void HandleChildCompleted(BTParallelNode self, BTRuntimeNode child, BTNodeState state)
        {
            BTParallelNodeData definition = self.Definition as BTParallelNodeData;
            if (definition == null)
            {
                self.Fail();
                return;
            }

            ++self.CompletedCount;
            if (state == BTNodeState.Success)
            {
                ++self.SuccessCount;
            }
            else
            {
                ++self.FailureCount;
            }

            if (definition.SuccessPolicy == BTParallelPolicy.RequireOne && state == BTNodeState.Success)
            {
                StopOtherChildren(self, child);
                self.Succeed();
                return;
            }

            if (definition.FailurePolicy == BTParallelPolicy.RequireOne && state != BTNodeState.Success)
            {
                StopOtherChildren(self, child);
                self.Fail();
                return;
            }

            if (self.CompletedCount < self.Children.Count)
            {
                return;
            }

            bool success = definition.SuccessPolicy == BTParallelPolicy.RequireAll ? self.SuccessCount == self.Children.Count : self.SuccessCount > 0;
            bool failure = definition.FailurePolicy == BTParallelPolicy.RequireAll ? self.FailureCount == self.Children.Count : self.FailureCount > 0;
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

        private static void StopOtherChildren(BTParallelNode self, BTRuntimeNode completedChild)
        {
            foreach (BTRuntimeNode child in self.Children)
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
