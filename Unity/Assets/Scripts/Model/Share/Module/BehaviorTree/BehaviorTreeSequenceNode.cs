namespace ET
{
    [EnableClass]
    internal sealed class BehaviorTreeSequenceNode : BehaviorTreeRuntimeNode
    {
        private int currentIndex;

        public BehaviorTreeSequenceNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
        {
        }

        protected override void OnStart()
        {
            this.currentIndex = 0;
            this.StartNextChild();
        }

        protected override void HandleChildCompleted(BehaviorTreeRuntimeNode child, BehaviorTreeNodeState state)
        {
            if (state != BehaviorTreeNodeState.Success)
            {
                this.Fail();
                return;
            }

            ++this.currentIndex;
            this.StartNextChild();
        }

        private void StartNextChild()
        {
            if (this.currentIndex >= this.Children.Count)
            {
                this.Succeed();
                return;
            }

            this.Children[this.currentIndex].Start();
        }
    }
}
