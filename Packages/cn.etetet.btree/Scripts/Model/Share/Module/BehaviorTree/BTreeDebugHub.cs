using System.Collections.Generic;

namespace ET
{
    [CodeProcess]
    [AllowInstance]
    public class BTreeDebugHub : Singleton<BTreeDebugHub>, ISingletonAwake
    {
        public readonly Dictionary<long, BTreeDebugSnapshot> Snapshots = new();

        public void Awake()
        {
        }
    }
}
