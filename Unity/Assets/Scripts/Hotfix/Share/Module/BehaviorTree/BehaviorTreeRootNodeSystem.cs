namespace ET
{
    public static partial class BehaviorTreeRuntimeNodeSystem
    {
        private static void OnStart(BehaviorTreeRootNode self)
        {
            if (self.Children.Count == 0)
            {
                self.Fail();
                return;
            }

            self.Children[0].Start();
        }

        private static void HandleChildCompleted(BehaviorTreeRootNode self, BehaviorTreeRuntimeNode child, BehaviorTreeNodeState state)
        {
            if (state == BehaviorTreeNodeState.Success)
            {
                self.Succeed();
                return;
            }

            self.Fail();
        }
    }
}
