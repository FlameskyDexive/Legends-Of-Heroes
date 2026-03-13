using System.Collections.Generic;

namespace ET
{
    [Code]
    public class BehaviorTreeRuntimeManager : Singleton<BehaviorTreeRuntimeManager>, ISingletonAwake
    {
        public readonly Dictionary<long, BehaviorTreeRunner> Runners = new();

        public void Awake()
        {
        }
    }
}
