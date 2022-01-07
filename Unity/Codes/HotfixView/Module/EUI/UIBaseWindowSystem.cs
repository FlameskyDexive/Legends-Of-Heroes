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
        /// <summary>
        /// 同步加载
        /// </summary>
        public static void Load(this UIBaseWindow self)
        {
            ResourcesComponent resourcesComponent = Game.Scene.GetComponent<ResourcesComponent>();
            if ( !UIGlobalDefine.WindowPrefabPath.TryGetValue((int)self.WindowID,out string value) )
            {
                Log.Error($"{self.WindowID} is not Exist!");
                return;
            }
            ResourcesComponent.Instance.LoadBundle(value.StringToAB());
            GameObject go                  = resourcesComponent.GetAsset(value.StringToAB(), value ) as GameObject;
            self.m_uiPrefabGameObject      = UnityEngine.Object.Instantiate(go);
            self.m_uiPrefabGameObject.name = go.name;
        }

        /// <summary>
        /// 异步加载
        /// </summary>
        public static async ETTask LoadAsync(this UIBaseWindow self)
        {
            ResourcesComponent resourcesComponent = Game.Scene.GetComponent<ResourcesComponent>();
            if ( !UIGlobalDefine.WindowPrefabPath.TryGetValue((int)self.WindowID,out string value) )
            {
                Log.Error($"{self.WindowID} is not Exist!");
                return;
            }
            await ResourcesComponent.Instance.LoadBundleAsync(value.StringToAB());
            GameObject go                  = resourcesComponent.GetAsset(value.StringToAB(), value ) as GameObject;
            self.m_uiPrefabGameObject      = UnityEngine.Object.Instantiate(go);
            self.m_uiPrefabGameObject.name = go.name;
        }
        
        public static void SetRoot(this UIBaseWindow self, Transform rootTransform)
        {
            if (null != rootTransform && null != self.uiTransform)
            {
                self.uiTransform.SetParent(rootTransform, false);
                self.uiTransform.transform.localScale = Vector3.one;
            }
        }
    }
}
