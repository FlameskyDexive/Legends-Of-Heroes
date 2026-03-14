using System.Collections.Generic;

namespace ET
{
    [Code]
    public class BTRuntimeManager : Singleton<BTRuntimeManager>, ISingletonAwake
    {
        public readonly Dictionary<long, BTRunner> Runners = new();

        public void Awake()
        {
        }
    }
}
