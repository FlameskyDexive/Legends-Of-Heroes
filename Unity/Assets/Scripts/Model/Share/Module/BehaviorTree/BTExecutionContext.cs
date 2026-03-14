namespace ET
{
    [EnableClass]
    public sealed class BTExecutionContext
    {
        public EntityRef<Entity> Owner;

        public long RuntimeId;

        public string TreeId = string.Empty;

        public string TreeName = string.Empty;

        public BTBlackboard Blackboard;

        public BTExecutionContext()
        {
        }

        public BTExecutionContext(long runtimeId, string treeId, string treeName, Entity owner, BTBlackboard blackboard)
        {
            this.RuntimeId = runtimeId;
            this.TreeId = treeId;
            this.TreeName = treeName;
            this.Owner = owner;
            this.Blackboard = blackboard;
        }
    }
}
