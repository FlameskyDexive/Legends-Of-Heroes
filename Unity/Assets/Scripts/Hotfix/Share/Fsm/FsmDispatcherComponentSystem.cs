using System;
using System.Collections.Generic;

namespace ET
{

    [EntitySystemOf(typeof(FsmDispatcherComponent))]
    [FriendOf(typeof(FsmDispatcherComponent))]
    public static partial class FsmDispatcherComponentSystem
    {
        [EntitySystem]
        public static void Awake(this FsmDispatcherComponent self)
        {
            FsmDispatcherComponent.Instance = self;
            self.FsmNodeHandlers.Clear();

            var types = EventSystem.Instance.GetTypes(typeof (FsmNodeAttribute));
            foreach (Type type in types)
            {
                AFsmNodeHandler aFsmNodeHandler = Activator.CreateInstance(type) as AFsmNodeHandler;
                if (aFsmNodeHandler == null)
                {
                    Log.Error("{0} is not AFsmNode.".Fmt(type.Name));
                    continue;
                }

                aFsmNodeHandler.Name = type.Name;
                self.FsmNodeHandlers.Add(type.Name, aFsmNodeHandler);
            }
        }
        [EntitySystem]
        public static void Destroy(this FsmDispatcherComponent self)
        {
            self.FsmNodeHandlers.Clear();
            FsmDispatcherComponent.Instance = null;
            
        }
    }
}