namespace ET
{
    [EntitySystemOf(typeof(BTComponent))]
    [FriendOf(typeof(BTComponent))]
    public static partial class BTComponentSystem
    {
        [EntitySystem]
        private static void Awake(this BTComponent self, byte[] treeBytes, string treeIdOrName)
        {
            self.TreeBytes = treeBytes;
            self.TreeIdOrName = treeIdOrName;
            self.StartTree();
        }

        [EntitySystem]
        private static void Destroy(this BTComponent self)
        {
            self.StopTree();
            self.TreeBytes = null;
            self.TreeIdOrName = null;
            self.BlackboardOverrides.Clear();
        }

        public static void Restart(this BTComponent self)
        {
            self.StopTree();
            self.StartTree();
        }

        public static void Reload(this BTComponent self, byte[] treeBytes, string treeIdOrName = "")
        {
            self.TreeBytes = treeBytes;
            self.TreeIdOrName = treeIdOrName;
            self.Restart();
        }

        public static void SetBlackboardValue(this BTComponent self, string key, object value)
        {
            BTRunner runner = self.GetRunner();
            runner?.Blackboard.SetBoxed(key, value);
        }

        public static BTRunner GetRunner(this BTComponent self)
        {
            return BTRuntimeManager.Instance.Get(self.RuntimeId);
        }

        private static void StartTree(this BTComponent self)
        {
            if (self.TreeBytes == null || self.TreeBytes.Length == 0)
            {
                Log.Warning($"behavior tree bytes empty: {self.Id}");
                return;
            }

            Unit unit = self.GetParent<Unit>();
            BTRunner runner = BTRuntime.Create(unit, self.TreeBytes, self.TreeIdOrName);
            if (runner == null)
            {
                Log.Error($"behavior tree create failed: {self.TreeIdOrName}");
                return;
            }

            foreach ((string key, BTSerializedValue value) in self.BlackboardOverrides)
            {
                runner.Blackboard.SetBoxed(key, BTValueUtility.GetValue(value));
            }

            runner.Start();
            self.RuntimeId = runner.RuntimeId;
            BTRuntimeManager.Instance.Add(self.RuntimeId, runner);
        }

        private static void StopTree(this BTComponent self)
        {
            BTRunner runner = BTRuntimeManager.Instance.Remove(self.RuntimeId);
            self.RuntimeId = 0;
            runner?.Dispose();
        }
    }
}
