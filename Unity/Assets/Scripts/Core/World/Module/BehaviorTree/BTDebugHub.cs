using System.Collections.Generic;

namespace ET
{
    [Code]
    public class BTDebugHub : Singleton<BTDebugHub>, ISingletonAwake
    {
        public readonly Dictionary<long, BTDebugSnapshot> Snapshots = new();

        public void Awake()
        {
        }
    }
}
