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

        private static void HandleChildCompleted(BTInverterNode self, BTRuntimeNode child, BTNodeState state)
        {
            if (state == BTNodeState.Success)
            {
                self.Fail();
                return;
            }

            self.Succeed();
        }
    }
}
