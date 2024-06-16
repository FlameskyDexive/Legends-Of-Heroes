using System;

namespace ET.Client
{
    [EntitySystemOf(typeof(UIEventComponent))]
    [FriendOf(typeof(UIEventComponent))]
    public static partial class UIEventComponentSystem
    {
        [EntitySystem]
        private static void Awake(this UIEventComponent self)
        {
            self.UIEventHandlers.Clear();

            var AUIEventAttributeSets =  CodeTypes.Instance.GetTypes(typeof (AUIEventAttribute));
            foreach (Type v in AUIEventAttributeSets)
            {
                AUIEventAttribute attr = v.GetCustomAttributes(typeof (AUIEventAttribute), false)[0] as AUIEventAttribute;
                self.UIEventHandlers.Add(attr.WindowID, Activator.CreateInstance(v) as IAUIEventHandler);
            }
        }
        
        [EntitySystem]
        private static  void Destroy(this UIEventComponent self)
        {
            self.UIEventHandlers.Clear();
            self.IsClicked = false;
        }
        
        public static IAUIEventHandler GetUIEventHandler(this UIEventComponent self,WindowID windowID)
        {
            if (self.UIEventHandlers.TryGetValue(windowID, out IAUIEventHandler handler))
            {
                return handler;
            }
            Log.Error($"windowId : {windowID} is not have any uiEvent");
            return null;
        }

        public static void SetUIClicked(this UIEventComponent self, bool isClicked)
        {
            self.IsClicked = isClicked;
        }
        
    }
}