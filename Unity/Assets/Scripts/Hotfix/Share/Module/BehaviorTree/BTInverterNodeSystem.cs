namespace ET
{
    public static partial class BTRuntimeNodeSystem
    {
        private static void OnStart(BTInverterNode self)
        {
            if (self.Children.Count == 0)
            {
                self.Fail();
                return;
            }

            self.Children[0].Start();
        }

        private static void HandleChildCompleted(BTInverterNode self, BTRuntimeNode child, BehaviorTreeNodeState state)
        {
            if (state == BehaviorTreeNodeState.Success)
            {
                self.Fail();
                return;
            }

            self.Succeed();
        }
    }
}
