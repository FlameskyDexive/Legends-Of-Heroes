using System;
using System.Collections.Generic;

namespace ET
{
    [CodeProcess]
    [AllowInstance]
    public class BTreeActionDispatcher : Singleton<BTreeActionDispatcher>, ISingletonAwake
    {
        public readonly Dictionary<string, ABTreeActionHandler> Handlers = new(StringComparer.OrdinalIgnoreCase);

        public bool IsInitialized;

        public void Awake()
        {
        }
    }

    [CodeProcess]
    [AllowInstance]
    public class BTreeConditionDispatcher : Singleton<BTreeConditionDispatcher>, ISingletonAwake
    {
        public readonly Dictionary<string, ABTreeConditionHandler> Handlers = new(StringComparer.OrdinalIgnoreCase);

        public bool IsInitialized;

        public void Awake()
        {
        }
    }

    [CodeProcess]
    [AllowInstance]
    public class BTreeServiceDispatcher : Singleton<BTreeServiceDispatcher>, ISingletonAwake
    {
        public readonly Dictionary<string, ABTreeServiceHandler> Handlers = new(StringComparer.OrdinalIgnoreCase);

        public bool IsInitialized;

        public void Awake()
        {
        }
    }
}
