namespace ET
{
    [EnableClass]
    internal sealed class BehaviorTreeSubTreeNode : BehaviorTreeRuntimeNode
    {
        private BehaviorTreeRuntimeNode subTree;

        public BehaviorTreeSubTreeNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
        {
        }

        protected override void OnStart()
        {
            this.subTree = this.Runner.CreateSubTree(this.Definition.SubTreeId, this.Definition.SubTreeName, this);
            if (this.subTree == null)
            {
                Log.Error($"behavior tree subtree not found: {this.Definition.SubTreeId}/{this.Definition.SubTreeName}");
                this.Fail();
                return;
            }

            this.subTree.Start();
        }

        protected override void OnStop()
        {
            this.subTree?.Stop();
            this.subTree = null;
            base.OnStop();
        }

        protected override void HandleChildCompleted(BehaviorTreeRuntimeNode child, BehaviorTreeNodeState state)
        {
            this.subTree = null;
            if (state == BehaviorTreeNodeState.Success)
            {
                this.Succeed();
                return;
            }

            this.Fail();
        }
    }
}
