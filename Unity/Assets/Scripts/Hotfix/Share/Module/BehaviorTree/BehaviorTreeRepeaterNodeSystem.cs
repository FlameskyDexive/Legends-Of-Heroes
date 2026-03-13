namespace ET
{
    public static partial class BehaviorTreeRuntimeNodeSystem
    {
        private static void OnStart(BehaviorTreeRepeaterNode self)
        {
            self.CurrentLoopCount = 0;
            if (self.Children.Count == 0)
            {
                self.Succeed();
                return;
            }

            self.Children[0].Start();
        }

        private static void HandleChildCompleted(BehaviorTreeRepeaterNode self, BehaviorTreeRuntimeNode child, BehaviorTreeNodeState state)
        {
            ++self.CurrentLoopCount;
            if (self.Definition.MaxLoopCount > 0 && self.CurrentLoopCount >= self.Definition.MaxLoopCount)
            {
                if (state == BehaviorTreeNodeState.Success)
                {
                    self.Succeed();
                    return;
                }

                self.Fail();
                return;
            }

            child.Start();
        }
    }
}
