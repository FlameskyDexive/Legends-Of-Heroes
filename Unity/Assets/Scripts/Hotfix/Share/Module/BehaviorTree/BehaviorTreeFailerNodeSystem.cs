namespace ET
{
    public static partial class BehaviorTreeRuntimeNodeSystem
    {
        private static void OnStart(BehaviorTreeFailerNode self)
        {
            if (self.Children.Count == 0)
            {
                self.Fail();
                return;
            }

            self.Children[0].Start();
        }

        private static void HandleChildCompleted(BehaviorTreeFailerNode self, BehaviorTreeRuntimeNode child, BehaviorTreeNodeState state)
        {
            self.Fail();
        }
    }
}
