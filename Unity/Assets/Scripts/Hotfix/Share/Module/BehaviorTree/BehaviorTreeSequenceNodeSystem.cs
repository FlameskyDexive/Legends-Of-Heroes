namespace ET
{
    public static partial class BehaviorTreeRuntimeNodeSystem
    {
        private static void OnStart(BehaviorTreeSequenceNode self)
        {
            self.CurrentIndex = 0;
            StartNextSequenceChild(self);
        }

        private static void HandleChildCompleted(BehaviorTreeSequenceNode self, BehaviorTreeRuntimeNode child, BehaviorTreeNodeState state)
        {
            if (state != BehaviorTreeNodeState.Success)
            {
                self.Fail();
                return;
            }

            ++self.CurrentIndex;
            StartNextSequenceChild(self);
        }

        private static void StartNextSequenceChild(BehaviorTreeSequenceNode self)
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
