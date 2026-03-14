namespace ET
{
    public static partial class BTRuntimeNodeSystem
    {
        private static void OnStart(BTRootNode self)
        {
            if (self.Children.Count == 0)
            {
                self.Fail();
                return;
            }

            self.Children[0].Start();
        }

        private static void HandleChildCompleted(BTRootNode self, BTRuntimeNode child, BehaviorTreeNodeState state)
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
