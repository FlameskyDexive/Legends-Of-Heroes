using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public sealed class BehaviorTreeDebugSnapshot
    {
        public long RuntimeId;
        public string TreeId = string.Empty;
        public string TreeName = string.Empty;
        public long OwnerInstanceId;
        public long UpdatedAt;
        public Dictionary<string, BehaviorTreeNodeState> NodeStates = new();
    }
}
