namespace ET
{
    public static partial class BTRuntimeNodeSystem
    {
        private static void OnStart(BTSelectorNode self)
        {
            self.CurrentIndex = 0;
            StartNextSelectorChild(self);
        }

        private static void HandleChildCompleted(BTSelectorNode self, BTRuntimeNode child, BehaviorTreeNodeState state)
        {
            if (state == BehaviorTreeNodeState.Success)
            {
                self.Succeed();
                return;
            }

            ++self.CurrentIndex;
            StartNextSelectorChild(self);
        }

        private static void StartNextSelectorChild(BTSelectorNode self)
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
