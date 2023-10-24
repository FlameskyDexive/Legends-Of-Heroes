using System;

namespace ET.Client
{
    [EntitySystemOf(typeof(UIPathComponent))]
    [FriendOf(typeof(UIPathComponent))]
    public static partial class UIPathComponentSystem
    {
        [EntitySystem]
        public static void Awake(this UIPathComponent self)
        {
            foreach (WindowID windowID in Enum.GetValues(typeof(WindowID)))
            {
                string dlgName = "Dlg" + windowID.ToString().Split('_')[1];
                self.WindowPrefabPath.Add((int)windowID, dlgName);
                self.WindowTypeIdDict.Add(dlgName, (int)windowID);
            }
        }
        
        
        [EntitySystem]
        private static void Destroy(this ET.Client.UIPathComponent self)
        {
            self.WindowPrefabPath.Clear();
            self.WindowTypeIdDict.Clear();
        }
    }
}