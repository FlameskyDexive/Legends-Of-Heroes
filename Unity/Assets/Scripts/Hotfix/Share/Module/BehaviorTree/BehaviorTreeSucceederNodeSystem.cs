namespace ET
{
    public static partial class BehaviorTreeRuntimeNodeSystem
    {
        private static void OnStart(BehaviorTreeSucceederNode self)
        {
            if (self.Children.Count == 0)
            {
                self.Succeed();
                return;
            }

            self.Children[0].Start();
        }

        private static void HandleChildCompleted(BehaviorTreeSucceederNode self, BehaviorTreeRuntimeNode child, BehaviorTreeNodeState state)
        {
            self.Succeed();
        }
    }
}
