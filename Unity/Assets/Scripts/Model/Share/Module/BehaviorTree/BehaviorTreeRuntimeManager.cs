using System.Collections.Generic;

namespace ET
{
    [Code]
    public class BehaviorTreeRuntimeManager : Singleton<BehaviorTreeRuntimeManager>, ISingletonAwake
    {
        private readonly Dictionary<long, BehaviorTreeRunner> runners = new();

        public void Awake()
        {
        }

        public void Add(long runtimeId, BehaviorTreeRunner runner)
        {
            if (runtimeId == 0 || runner == null)
            {
                return;
            }

            this.runners[runtimeId] = runner;
        }

        public BehaviorTreeRunner Remove(long runtimeId)
        {
            if (runtimeId == 0)
            {
                return null;
            }

            this.runners.Remove(runtimeId, out BehaviorTreeRunner runner);
            return runner;
        }

        public BehaviorTreeRunner Get(long runtimeId)
        {
            if (runtimeId == 0)
            {
                return null;
            }

            this.runners.TryGetValue(runtimeId, out BehaviorTreeRunner runner);
            return runner;
        }
    }
}
