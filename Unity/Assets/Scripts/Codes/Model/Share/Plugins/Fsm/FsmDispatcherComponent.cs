using System.Collections.Generic;

namespace ET
{
    [ComponentOf(typeof(Scene))]
    public class FsmDispatcherComponent: Entity, IAwake, IDestroy, ILoad
    {
        [StaticField]
        public static FsmDispatcherComponent Instance;

        public Dictionary<string, AFsmNodeHandler> FsmNodeHandlers = new Dictionary<string, AFsmNodeHandler>();
    }
}