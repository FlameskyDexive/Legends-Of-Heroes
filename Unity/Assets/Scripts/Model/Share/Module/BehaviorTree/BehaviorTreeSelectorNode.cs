namespace ET
{
    [EnableClass]
    internal sealed class BehaviorTreeSelectorNode : BehaviorTreeRuntimeNode
    {
        private int currentIndex;

        public BehaviorTreeSelectorNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
        {
        }

        protected override void OnStart()
        {
            this.currentIndex = 0;
            this.StartNextChild();
        }

        protected override void HandleChildCompleted(BehaviorTreeRuntimeNode child, BehaviorTreeNodeState state)
        {
            if (state == BehaviorTreeNodeState.Success)
            {
                this.Succeed();
                return;
            }

            ++this.currentIndex;
            this.StartNextChild();
        }

        private void StartNextChild()
        {
            if (this.currentIndex >= this.Children.Count)
            {
                this.Fail();
                return;
            }

            this.Children[this.currentIndex].Start();
        }
    }
}
