using System.Collections.Generic;

namespace ET
{
    public static class BTDebugHubSystem
    {
        public static void Publish(this BTDebugHub self, long runtimeId, string treeId, string treeName, long ownerInstanceId,
            Dictionary<string, BTNodeState> nodeStates, Dictionary<string, string> blackboardValues)
        {
            BTDebugSnapshot snapshot = new()
            {
                RuntimeId = runtimeId,
                TreeId = treeId ?? string.Empty,
                TreeName = treeName ?? string.Empty,
                OwnerInstanceId = ownerInstanceId,
                UpdatedAt = TimeInfo.Instance.ServerNow(),
                NodeStates = new Dictionary<string, BTNodeState>(nodeStates),
                BlackboardValues = new Dictionary<string, string>(blackboardValues),
            };

            self.Snapshots[runtimeId] = snapshot;
        }

        public static void Remove(this BTDebugHub self, long runtimeId)
        {
            self.Snapshots.Remove(runtimeId);
        }

        public static List<BTDebugSnapshot> GetSnapshots(this BTDebugHub self, string treeId)
        {
            List<BTDebugSnapshot> snapshots = new();
            foreach (BTDebugSnapshot snapshot in self.Snapshots.Values)
            {
                if (snapshot.TreeId == treeId)
                {
                    snapshots.Add(snapshot);
                }
            }

            snapshots.Sort((left, right) => right.UpdatedAt.CompareTo(left.UpdatedAt));
            return snapshots;
        }

        public static BTDebugSnapshot GetSnapshot(this BTDebugHub self, long runtimeId)
        {
            self.Snapshots.TryGetValue(runtimeId, out BTDebugSnapshot snapshot);
            return snapshot;
        }
    }
}
