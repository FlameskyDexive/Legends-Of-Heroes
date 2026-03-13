namespace ET
{
    public static partial class BehaviorTreeRuntimeNodeSystem
    {
        private static void OnStart(BehaviorTreeInverterNode self)
        {
            if (self.Children.Count == 0)
            {
                self.Fail();
                return;
            }

            self.Children[0].Start();
        }

        private static void HandleChildCompleted(BehaviorTreeInverterNode self, BehaviorTreeRuntimeNode child, BehaviorTreeNodeState state)
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
