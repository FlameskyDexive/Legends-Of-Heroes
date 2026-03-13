using System.Collections.Generic;

namespace ET
{
    [Code]
    public class BehaviorTreeDebugHub : Singleton<BehaviorTreeDebugHub>, ISingletonAwake
    {
        public readonly Dictionary<long, BehaviorTreeDebugSnapshot> Snapshots = new();

        public void Awake()
        {
        }
    }
}
