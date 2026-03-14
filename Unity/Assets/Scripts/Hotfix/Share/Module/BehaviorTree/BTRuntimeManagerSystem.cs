namespace ET
{
    public static class BTRuntimeManagerSystem
    {
        public static void Add(this BTRuntimeManager self, long runtimeId, BTRunner runner)
        {
            if (runtimeId == 0 || runner == null)
            {
                return;
            }

            self.Runners[runtimeId] = runner;
        }

        public static BTRunner Remove(this BTRuntimeManager self, long runtimeId)
        {
            if (runtimeId == 0)
            {
                return null;
            }

            self.Runners.Remove(runtimeId, out BTRunner runner);
            return runner;
        }

        public static BTRunner Get(this BTRuntimeManager self, long runtimeId)
        {
            if (runtimeId == 0)
            {
                return null;
            }

            self.Runners.TryGetValue(runtimeId, out BTRunner runner);
            return runner;
        }
    }
}
