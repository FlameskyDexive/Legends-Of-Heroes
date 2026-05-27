using System;

namespace ET.Client
{
    [EntitySystemOf(typeof(UIPathComponent))]
    [FriendOf(typeof(UIPathComponent))]
    public static partial class UIPathComponentSystem
    {
        [EntitySystem]
        private static void Awake(this UIPathComponent self)
        {
            foreach (WindowID windowID in Enum.GetValues(typeof(WindowID)))
            {
                if (windowID == WindowID.WindowID_Invaild)
                {
                    continue;
                }
                string dlgName = "Dlg" + windowID.ToString().Split('_')[1];
                self.WindowPrefabPath[(int)windowID] = dlgName;
                self.WindowTypeIdDict[dlgName] = (int)windowID;
            }
        }

        [EntitySystem]
        private static void Destroy(this UIPathComponent self)
        {
            self.WindowPrefabPath.Clear();
            self.WindowTypeIdDict.Clear();
        }
    }
}
