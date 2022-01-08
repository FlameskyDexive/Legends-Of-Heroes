using UnityEngine;

namespace ET
{
    
    [ObjectSystem]
    public class UIBaseWindowAwakeSystem : AwakeSystem<UIBaseWindow>
    {
        public override void Awake(UIBaseWindow self)
        {
            self.WindowData = self.AddChild<WindowCoreData>();
        }
    }
    
    public  static class UIBaseWindowSystem  
    {
        public static void SetRoot(this UIBaseWindow self, Transform rootTransform)
        {
            if(self.uiTransform == null)
            {
                Log.Error($"uibaseWindows {self.WindowID} uiTransform is null!!!");
                return;
            }
            if(rootTransform == null)
            {
                Log.Error($"uibaseWindows {self.WindowID} rootTransform is null!!!");
                return;
            }
            self.uiTransform.SetParent(rootTransform, false);
            self.uiTransform.transform.localScale = Vector3.one;
        }
    }
}
