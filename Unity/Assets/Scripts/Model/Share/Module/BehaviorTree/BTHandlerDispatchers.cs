using System;
using System.Collections.Generic;

namespace ET
{
    [Code]
    public class BTActionDispatcher : Singleton<BTActionDispatcher>, ISingletonAwake
    {
        public readonly Dictionary<string, ABTActionHandler> Handlers = new(StringComparer.OrdinalIgnoreCase);

        public bool IsInitialized;

        public void Awake()
        {
        }
    }

    [Code]
    public class BTConditionDispatcher : Singleton<BTConditionDispatcher>, ISingletonAwake
    {
        public readonly Dictionary<string, ABTConditionHandler> Handlers = new(StringComparer.OrdinalIgnoreCase);

        public bool IsInitialized;

        public void Awake()
        {
        }
    }

    [Code]
    public class BTServiceDispatcher : Singleton<BTServiceDispatcher>, ISingletonAwake
    {
        public readonly Dictionary<string, ABTServiceHandler> Handlers = new(StringComparer.OrdinalIgnoreCase);

        public bool IsInitialized;

        public void Awake()
        {
        }
    }
}
