namespace ET
{
    public static class BehaviorTreeRuntimeManagerSystem
    {
        public static void Add(this BehaviorTreeRuntimeManager self, long runtimeId, BehaviorTreeRunner runner)
        {
            if (runtimeId == 0 || runner == null)
            {
                return;
            }

            self.Runners[runtimeId] = runner;
        }

        public static BehaviorTreeRunner Remove(this BehaviorTreeRuntimeManager self, long runtimeId)
        {
            if (runtimeId == 0)
            {
                return null;
            }

            self.Runners.Remove(runtimeId, out BehaviorTreeRunner runner);
            return runner;
        }

        public static BehaviorTreeRunner Get(this BehaviorTreeRuntimeManager self, long runtimeId)
        {
            if (runtimeId == 0)
            {
                return null;
            }

            self.Runners.TryGetValue(runtimeId, out BehaviorTreeRunner runner);
            return runner;
        }
    }
}
