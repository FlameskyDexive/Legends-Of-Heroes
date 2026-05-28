namespace ET
{
    [EntitySystemOf(typeof(BTreeComponent))]
    [FriendOf(typeof(BTreeComponent))]
    public static partial class BTreeComponentSystem
    {
        [EntitySystem]
        private static void Awake(this BTreeComponent self, byte[] treeBytes, string treeIdOrName)
        {
            self.TreeBytes = treeBytes;
            self.TreeIdOrName = treeIdOrName;
            self.StartTree();
        }

        [EntitySystem]
        private static void Destroy(this BTreeComponent self)
        {
            self.StopTree();
            self.TreeBytes = null;
            self.TreeIdOrName = null;
            self.BlackboardOverrides.Clear();
        }

        public static void Restart(this BTreeComponent self)
        {
            self.StopTree();
            self.StartTree();
        }

        public static void Reload(this BTreeComponent self, byte[] treeBytes, string treeIdOrName = "")
        {
            self.TreeBytes = treeBytes;
            self.TreeIdOrName = treeIdOrName;
            self.Restart();
        }

        public static void SetBlackboardValue(this BTreeComponent self, string key, object value)
        {
            BTreeExecutionSession session = self.GetSession();
            session?.Blackboard.SetBoxed(key, value);
        }

        public static BTreeExecutionSession GetSession(this BTreeComponent self)
        {
            return BTreeExecutionSessionManager.Instance.Get(self.RuntimeId);
        }

        private static void StartTree(this BTreeComponent self)
        {
            if (self.TreeBytes == null || self.TreeBytes.Length == 0)
            {
                Log.Warning($"behavior tree bytes empty: {self.Id}");
                return;
            }

            Unit unit = self.GetParent<Unit>();
            BTreeExecutionSession session = BTreeRuntime.Create(unit, self.TreeBytes, self.TreeIdOrName);
            if (session == null)
            {
                Log.Error($"behavior tree create failed: {self.TreeIdOrName}");
                return;
            }

            foreach ((string key, BTreeSerializedValue value) in self.BlackboardOverrides)
            {
                session.Blackboard.SetBoxed(key, BTreeValueUtility.GetValue(value));
            }

            BTreeFlowDriver.RunRoot(session);
            self.RuntimeId = session.RuntimeId;
            BTreeExecutionSessionManager.Instance.Add(session);
        }

        private static void StopTree(this BTreeComponent self)
        {
            BTreeExecutionSession session = BTreeExecutionSessionManager.Instance.Remove(self.RuntimeId);
            self.RuntimeId = 0;
            BTreeFlowDriver.Dispose(session);
        }
    }
}
