using System;

namespace ET
{
    [FriendOf(typeof(UIPathComponent))]
    [EntitySystemOf(typeof(UIPathComponent))]
    public static class UIPathComponentSystem
    {
        public static void Destroy(this UIPathComponent self)
        {
            self.WindowPrefabPath.Clear();
            self.WindowTypeIdDict.Clear();
            UIPathComponent.Instance = null; 
        }
        public static void Awake(this UIPathComponent self)
        {
            UIPathComponent.Instance = self;
            foreach (WindowID windowID in Enum.GetValues(typeof(WindowID)))
            {
                string dlgName = "Dlg" + windowID.ToString().Split('_')[1];
                self.WindowPrefabPath.Add((int)windowID , dlgName);
                self.WindowTypeIdDict.Add(dlgName, (int)windowID );
            }
        }
    }
}