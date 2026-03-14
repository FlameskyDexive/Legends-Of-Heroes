namespace ET
{
    public static partial class BTRuntimeNodeSystem
    {
        private static void OnStart(BTSubTreeNode self)
        {
            BTSubTreeNodeData definition = self.Definition as BTSubTreeNodeData;
            if (definition == null)
            {
                self.Fail();
                return;
            }

            self.SubTree = CreateSubTree(self.Runner, definition.SubTreeId, definition.SubTreeName, self);
            if (self.SubTree == null)
            {
                Log.Error($"behavior tree subtree not found: {definition.SubTreeId}/{definition.SubTreeName}");
                self.Fail();
                return;
            }

            self.SubTree.Start();
        }

        private static void HandleChildCompleted(BTSubTreeNode self, BTRuntimeNode child, BTNodeState state)
        {
            self.SubTree = null;
            if (state == BTNodeState.Success)
            {
                self.Succeed();
                return;
            }

            self.Fail();
        }

        private static void OnStop(BTSubTreeNode self)
        {
            self.SubTree?.Stop();
            self.SubTree = null;
            self.StopChildren();
        }
    }
}
