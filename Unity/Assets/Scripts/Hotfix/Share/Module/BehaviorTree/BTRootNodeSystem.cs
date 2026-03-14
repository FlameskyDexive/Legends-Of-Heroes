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

        private static void HandleChildCompleted(BTRootNode self, BTRuntimeNode child, BTNodeState state)
        {
            if (state == BTNodeState.Success)
            {
                self.Succeed();
                return;
            }

            self.Fail();
        }
    }
}
