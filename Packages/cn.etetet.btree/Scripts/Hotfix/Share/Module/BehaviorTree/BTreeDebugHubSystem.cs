using System.Collections.Generic;

namespace ET
{
    public static class BTreeDebugHubSystem
    {
        public static void Publish(this BTreeDebugHub self, long runtimeId, string treeId, string treeName, long ownerInstanceId,
            Dictionary<string, BTreeNodeState> nodeStates, Dictionary<string, string> blackboardValues, long updatedAt)
        {
            BTreeDebugSnapshot snapshot = new()
            {
                RuntimeId = runtimeId,
                TreeId = treeId ?? string.Empty,
                TreeName = treeName ?? string.Empty,
                OwnerInstanceId = ownerInstanceId,
                UpdatedAt = updatedAt,
                NodeStates = new Dictionary<string, BTreeNodeState>(nodeStates),
                BlackboardValues = new Dictionary<string, string>(blackboardValues),
            };

            self.Snapshots[runtimeId] = snapshot;
        }

        public static void Remove(this BTreeDebugHub self, long runtimeId)
        {
            self.Snapshots.Remove(runtimeId);
        }

        public static List<BTreeDebugSnapshot> GetSnapshots(this BTreeDebugHub self, string treeId)
        {
            List<BTreeDebugSnapshot> snapshots = new();
            foreach (BTreeDebugSnapshot snapshot in self.Snapshots.Values)
            {
                if (snapshot.TreeId == treeId)
                {
                    snapshots.Add(snapshot);
                }
            }

            snapshots.Sort((left, right) => right.UpdatedAt.CompareTo(left.UpdatedAt));
            return snapshots;
        }

        public static BTreeDebugSnapshot GetSnapshot(this BTreeDebugHub self, long runtimeId)
        {
            self.Snapshots.TryGetValue(runtimeId, out BTreeDebugSnapshot snapshot);
            return snapshot;
        }
    }
}
