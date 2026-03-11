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

    [Code]
    public class BehaviorTreeDebugHub : Singleton<BehaviorTreeDebugHub>, ISingletonAwake
    {
        private readonly Dictionary<long, BehaviorTreeDebugSnapshot> snapshots = new();

        public void Awake()
        {
        }

        public void Publish(long runtimeId, string treeId, string treeName, long ownerInstanceId, Dictionary<string, BehaviorTreeNodeState> nodeStates)
        {
            BehaviorTreeDebugSnapshot snapshot = new()
            {
                RuntimeId = runtimeId,
                TreeId = treeId ?? string.Empty,
                TreeName = treeName ?? string.Empty,
                OwnerInstanceId = ownerInstanceId,
                UpdatedAt = TimeInfo.Instance.ServerNow(),
                NodeStates = new Dictionary<string, BehaviorTreeNodeState>(nodeStates),
            };

            this.snapshots[runtimeId] = snapshot;
        }

        public void Remove(long runtimeId)
        {
            this.snapshots.Remove(runtimeId);
        }

        public List<BehaviorTreeDebugSnapshot> GetSnapshots(string treeId)
        {
            List<BehaviorTreeDebugSnapshot> snapshots = new();
            foreach (BehaviorTreeDebugSnapshot snapshot in this.snapshots.Values)
            {
                if (snapshot.TreeId == treeId)
                {
                    snapshots.Add(snapshot);
                }
            }

            snapshots.Sort((left, right) => right.UpdatedAt.CompareTo(left.UpdatedAt));
            return snapshots;
        }

        public BehaviorTreeDebugSnapshot GetSnapshot(long runtimeId)
        {
            this.snapshots.TryGetValue(runtimeId, out BehaviorTreeDebugSnapshot snapshot);
            return snapshot;
        }
    }
}
