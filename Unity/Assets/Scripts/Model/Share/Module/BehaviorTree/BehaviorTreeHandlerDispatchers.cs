using System;
using System.Collections.Generic;

namespace ET
{
    [Code]
    public class BehaviorTreeActionDispatcher : Singleton<BehaviorTreeActionDispatcher>, ISingletonAwake
    {
        public readonly Dictionary<string, ABehaviorTreeActionHandler> Handlers = new(StringComparer.OrdinalIgnoreCase);

        public bool IsInitialized;

        public void Awake()
        {
        }
    }

    [Code]
    public class BehaviorTreeConditionDispatcher : Singleton<BehaviorTreeConditionDispatcher>, ISingletonAwake
    {
        public readonly Dictionary<string, ABehaviorTreeConditionHandler> Handlers = new(StringComparer.OrdinalIgnoreCase);

        public bool IsInitialized;

        public void Awake()
        {
        }
    }

    [Code]
    public class BehaviorTreeServiceDispatcher : Singleton<BehaviorTreeServiceDispatcher>, ISingletonAwake
    {
        public readonly Dictionary<string, ABehaviorTreeServiceHandler> Handlers = new(StringComparer.OrdinalIgnoreCase);

        public bool IsInitialized;

        public void Awake()
        {
        }
    }
}
