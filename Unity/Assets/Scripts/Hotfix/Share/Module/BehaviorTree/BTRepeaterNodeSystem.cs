namespace ET
{
    public static partial class BTRuntimeNodeSystem
    {
        private static void OnStart(BTRepeaterNode self)
        {
            self.CurrentLoopCount = 0;
            if (self.Children.Count == 0)
            {
                self.Succeed();
                return;
            }

            self.Children[0].Start();
        }

        private static void HandleChildCompleted(BTRepeaterNode self, BTRuntimeNode child, BTNodeState state)
        {
            BTRepeaterNodeData definition = self.Definition as BTRepeaterNodeData;
            if (definition == null)
            {
                self.Fail();
                return;
            }

            ++self.CurrentLoopCount;
            if (definition.MaxLoopCount > 0 && self.CurrentLoopCount >= definition.MaxLoopCount)
            {
                if (state == BTNodeState.Success)
                {
                    self.Succeed();
                    return;
                }

                self.Fail();
                return;
            }

            child.Start();
        }
    }
}
