namespace ET
{
    public static partial class BTRuntimeNodeSystem
    {
        private static void OnStart(BTSequenceNode self)
        {
            self.CurrentIndex = 0;
            StartNextSequenceChild(self);
        }

        private static void HandleChildCompleted(BTSequenceNode self, BTRuntimeNode child, BehaviorTreeNodeState state)
        {
            if (state != BehaviorTreeNodeState.Success)
            {
                self.Fail();
                return;
            }

            ++self.CurrentIndex;
            StartNextSequenceChild(self);
        }

        private static void StartNextSequenceChild(BTSequenceNode self)
        {
            if (self.CurrentIndex >= self.Children.Count)
            {
                self.Succeed();
                return;
            }

            self.Children[self.CurrentIndex].Start();
        }
    }
}
