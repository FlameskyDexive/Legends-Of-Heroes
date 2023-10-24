using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(UIBaseWindow))]
    [FriendOfAttribute(typeof(ET.Client.UIBaseWindow))]
    public static partial class UIBaseWindowSystem
    {
        [EntitySystem]
        private static void Awake(this UIBaseWindow self)
        {
            self.IsInStackQueue = false;
        }

        [EntitySystem]
        private static void Destroy(this UIBaseWindow self)
        {
            self.WindowID = WindowID.WindowID_Invaild;
            self.IsInStackQueue = false;
            if (self.UIPrefabGameObject != null)
            {
                GameObject.Destroy(self.UIPrefabGameObject);
                self.UIPrefabGameObject = null;
            }
        }

        public static void SetRoot(this UIBaseWindow self, Transform rootTransform)
        {
            if (self.uiTransform == null)
            {
                Log.Error($"uibaseWindows {self.WindowID} uiTransform is null!!!");
                return;
            }
            if (rootTransform == null)
            {
                Log.Error($"uibaseWindows {self.WindowID} rootTransform is null!!!");
                return;
            }
            self.uiTransform.SetParent(rootTransform, false);
            self.uiTransform.transform.localScale = Vector3.one;
        }
    }
}
