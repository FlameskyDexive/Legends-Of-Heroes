namespace ET
{
    [EnableClass]
    public sealed class BehaviorTreeExecutionContext
    {
        public EntityRef<Entity> Owner;

        public long RuntimeId;

        public string TreeId = string.Empty;

        public string TreeName = string.Empty;

        public BehaviorTreeBlackboard Blackboard;

        public BehaviorTreeExecutionContext()
        {
        }

        public BehaviorTreeExecutionContext(long runtimeId, string treeId, string treeName, Entity owner, BehaviorTreeBlackboard blackboard)
        {
            this.RuntimeId = runtimeId;
            this.TreeId = treeId;
            this.TreeName = treeName;
            this.Owner = owner;
            this.Blackboard = blackboard;
        }
    }
}
