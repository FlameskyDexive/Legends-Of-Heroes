using System.Collections.Generic;

namespace ET
{
    public static class BehaviorTreeDebugHubSystem
    {
        public static void Publish(this BehaviorTreeDebugHub self, long runtimeId, string treeId, string treeName, long ownerInstanceId,
            Dictionary<string, BehaviorTreeNodeState> nodeStates)
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

            self.Snapshots[runtimeId] = snapshot;
        }

        public static void Remove(this BehaviorTreeDebugHub self, long runtimeId)
        {
            self.Snapshots.Remove(runtimeId);
        }

        public static List<BehaviorTreeDebugSnapshot> GetSnapshots(this BehaviorTreeDebugHub self, string treeId)
        {
            List<BehaviorTreeDebugSnapshot> snapshots = new();
            foreach (BehaviorTreeDebugSnapshot snapshot in self.Snapshots.Values)
            {
                if (snapshot.TreeId == treeId)
                {
                    snapshots.Add(snapshot);
                }
            }

            snapshots.Sort((left, right) => right.UpdatedAt.CompareTo(left.UpdatedAt));
            return snapshots;
        }

        public static BehaviorTreeDebugSnapshot GetSnapshot(this BehaviorTreeDebugHub self, long runtimeId)
        {
            self.Snapshots.TryGetValue(runtimeId, out BehaviorTreeDebugSnapshot snapshot);
            return snapshot;
        }
    }
}
