namespace ET
{
    public static partial class BTRuntimeNodeSystem
    {
        private static void OnStart(BTFailerNode self)
        {
            if (self.Children.Count == 0)
            {
                self.Fail();
                return;
            }

            self.Children[0].Start();
        }

        private static void HandleChildCompleted(BTFailerNode self, BTRuntimeNode child, BTNodeState state)
        {
            self.Fail();
        }
    }
}
