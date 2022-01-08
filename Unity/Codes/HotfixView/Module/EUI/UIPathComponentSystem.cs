using System;

namespace ET
{
    
    [ObjectSystem]
    public class UIPathComponentAwakeSystem : AwakeSystem<UIPathComponent>
    {
        public override void Awake(UIPathComponent self)
        {
            UIPathComponent.Instance = self;
            self.Awake();
        }
    }
    
    [ObjectSystem]
    public class UIPathComponentDestroySystem : DestroySystem<UIPathComponent>
    {
        public override void Destroy(UIPathComponent self)
        {
            self.WindowPrefabPath.Clear();
            self.WindowTypeIdDict.Clear();
            UIPathComponent.Instance = null;
        }
    }
    
    public static class UIPathComponentSystem
    {
        public static void Awake(this UIPathComponent self)
        {
            foreach (WindowID windowID in Enum.GetValues(typeof(WindowID)))
            {
                string dlgName = "Dlg" + windowID.ToString().Split('_')[1];
                self.WindowPrefabPath.Add((int)windowID , dlgName);
                self.WindowTypeIdDict.Add(dlgName, (int)windowID );
            }
        }
    }
}