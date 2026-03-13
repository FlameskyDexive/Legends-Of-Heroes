namespace ET
{
    public static partial class BehaviorTreeRuntimeNodeSystem
    {
        private static void OnStart(BehaviorTreeSubTreeNode self)
        {
            self.SubTree = CreateSubTree(self.Runner, self.Definition.SubTreeId, self.Definition.SubTreeName, self);
            if (self.SubTree == null)
            {
                Log.Error($"behavior tree subtree not found: {self.Definition.SubTreeId}/{self.Definition.SubTreeName}");
                self.Fail();
                return;
            }

            self.SubTree.Start();
        }

        private static void HandleChildCompleted(BehaviorTreeSubTreeNode self, BehaviorTreeRuntimeNode child, BehaviorTreeNodeState state)
        {
            self.SubTree = null;
            if (state == BehaviorTreeNodeState.Success)
            {
                self.Succeed();
                return;
            }

            self.Fail();
        }

        private static void OnStop(BehaviorTreeSubTreeNode self)
        {
            self.SubTree?.Stop();
            self.SubTree = null;
            self.StopChildren();
        }
    }
}
