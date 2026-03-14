namespace ET
{
    public static partial class BTRuntimeNodeSystem
    {
        private static void OnStart(BTSucceederNode self)
        {
            if (self.Children.Count == 0)
            {
                self.Succeed();
                return;
            }

            self.Children[0].Start();
        }

        private static void HandleChildCompleted(BTSucceederNode self, BTRuntimeNode child, BTNodeState state)
        {
            self.Succeed();
        }
    }
}
