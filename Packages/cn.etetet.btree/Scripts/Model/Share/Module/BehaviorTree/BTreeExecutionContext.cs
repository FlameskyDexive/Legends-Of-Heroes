namespace ET
{
    [EnableClass]
    public sealed class BTreeExecutionContext
    {
        public EntityRef<Entity> Owner;

        public long RuntimeId;

        public string TreeId = string.Empty;

        public string TreeName = string.Empty;

        public BTreeBlackboard Blackboard;

        public BTreeExecutionContext()
        {
        }

        public BTreeExecutionContext(long runtimeId, string treeId, string treeName, Entity owner, BTreeBlackboard blackboard)
        {
            this.RuntimeId = runtimeId;
            this.TreeId = treeId;
            this.TreeName = treeName;
            this.Owner = owner;
            this.Blackboard = blackboard;
        }

        public BTreeExecutionContext Configure(long runtimeId, string treeId, string treeName, Entity owner, BTreeBlackboard blackboard)
        {
            this.RuntimeId = runtimeId;
            this.TreeId = treeId ?? string.Empty;
            this.TreeName = treeName ?? string.Empty;
            this.Owner = owner;
            this.Blackboard = blackboard;
            return this;
        }
    }
}
