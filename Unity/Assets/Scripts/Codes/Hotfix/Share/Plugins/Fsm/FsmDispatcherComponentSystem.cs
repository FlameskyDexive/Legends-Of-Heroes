using System;
using System.Collections.Generic;

namespace ET
{
    [ObjectSystem]
    public class FsmDispatcherComponentLoadSystem: LoadSystem<FsmDispatcherComponent>
    {
        protected override void Load(FsmDispatcherComponent self)
        {
            self.Load();
        }
    }
    
    [ObjectSystem]
    public class FsmDispatcherComponentAwakeSystem: AwakeSystem<FsmDispatcherComponent>
    {
        protected override void Awake(FsmDispatcherComponent self)
        {
            FsmDispatcherComponent.Instance = self;
            self.Load();
        }
    }
    
    [ObjectSystem]
    public class FsmDispatcherComponentDestroySystem: DestroySystem<FsmDispatcherComponent>
    {
        protected override void Destroy(FsmDispatcherComponent self)
        {
            self.FsmNodeHandlers.Clear();
            FsmDispatcherComponent.Instance = null;
        }
    }
    
    [FriendOf(typeof(FsmDispatcherComponent))]
    public static class FsmDispatcherComponentSystem
    {
        public static void Load(this FsmDispatcherComponent self)
        {
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
    }
}