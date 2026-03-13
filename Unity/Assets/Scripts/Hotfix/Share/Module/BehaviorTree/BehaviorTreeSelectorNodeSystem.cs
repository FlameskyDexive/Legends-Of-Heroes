namespace ET
{
    public static partial class BehaviorTreeRuntimeNodeSystem
    {
        private static void OnStart(BehaviorTreeSelectorNode self)
        {
            self.CurrentIndex = 0;
            StartNextSelectorChild(self);
        }

        private static void HandleChildCompleted(BehaviorTreeSelectorNode self, BehaviorTreeRuntimeNode child, BehaviorTreeNodeState state)
        {
            if (state == BehaviorTreeNodeState.Success)
            {
                self.Succeed();
                return;
            }

            ++self.CurrentIndex;
            StartNextSelectorChild(self);
        }

        private static void StartNextSelectorChild(BehaviorTreeSelectorNode self)
        {
            if (self.CurrentIndex >= self.Children.Count)
            {
                self.Fail();
                return;
            }

            self.Children[self.CurrentIndex].Start();
        }
    }
}
