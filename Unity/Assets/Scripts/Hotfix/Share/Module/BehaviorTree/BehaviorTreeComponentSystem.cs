namespace ET
{
    [EntitySystemOf(typeof(BehaviorTreeComponent))]
    [FriendOf(typeof(BehaviorTreeComponent))]
    public static partial class BehaviorTreeComponentSystem
    {
        [EntitySystem]
        private static void Awake(this BehaviorTreeComponent self, byte[] treeBytes, string treeIdOrName)
        {
            self.TreeBytes = treeBytes;
            self.TreeIdOrName = treeIdOrName;
            self.StartTree();
        }

        [EntitySystem]
        private static void Destroy(this BehaviorTreeComponent self)
        {
            self.StopTree();
            self.TreeBytes = null;
            self.TreeIdOrName = null;
            self.BlackboardOverrides.Clear();
        }

        public static void Restart(this BehaviorTreeComponent self)
        {
            self.StopTree();
            self.StartTree();
        }

        public static void Reload(this BehaviorTreeComponent self, byte[] treeBytes, string treeIdOrName = "")
        {
            self.TreeBytes = treeBytes;
            self.TreeIdOrName = treeIdOrName;
            self.Restart();
        }

        public static void SetBlackboardValue(this BehaviorTreeComponent self, string key, object value)
        {
            BehaviorTreeRunner runner = self.GetRunner();
            runner?.Blackboard.SetBoxed(key, value);
        }

        public static BehaviorTreeRunner GetRunner(this BehaviorTreeComponent self)
        {
            return BehaviorTreeRuntimeManager.Instance.Get(self.RuntimeId);
        }

        private static void StartTree(this BehaviorTreeComponent self)
        {
            if (self.TreeBytes == null || self.TreeBytes.Length == 0)
            {
                Log.Warning($"behavior tree bytes empty: {self.Id}");
                return;
            }

            Unit unit = self.GetParent<Unit>();
            BehaviorTreeRunner runner = BehaviorTreeRuntime.Create(unit, self.TreeBytes, self.TreeIdOrName);
            if (runner == null)
            {
                Log.Error($"behavior tree create failed: {self.TreeIdOrName}");
                return;
            }

            foreach ((string key, BehaviorTreeSerializedValue value) in self.BlackboardOverrides)
            {
                runner.Blackboard.SetBoxed(key, BehaviorTreeValueUtility.GetValue(value));
            }

            runner.Start();
            self.RuntimeId = runner.RuntimeId;
            BehaviorTreeRuntimeManager.Instance.Add(self.RuntimeId, runner);
        }

        private static void StopTree(this BehaviorTreeComponent self)
        {
            BehaviorTreeRunner runner = BehaviorTreeRuntimeManager.Instance.Remove(self.RuntimeId);
            self.RuntimeId = 0;
            runner?.Dispose();
        }
    }
}
