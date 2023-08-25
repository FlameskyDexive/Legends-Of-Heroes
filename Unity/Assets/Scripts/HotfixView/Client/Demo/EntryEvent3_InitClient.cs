using System;
using System.Collections.Generic;
using System.IO;

namespace ET.Client
{
    [Event(SceneType.Main)]
    public class EntryEvent3_InitClient: AEvent<Scene, EntryEvent3>
    {
        protected override async ETTask Run(Scene root, EntryEvent3 args)
        {
            World.Instance.AddSingleton<UIEventComponent>();
            // root.AddComponent<CoroutineLockComponent>();
            
            GlobalComponent globalComponent = root.AddComponent<GlobalComponent>();
            // root.AddComponent<UIGlobalComponent>();
            root.AddComponent<UIComponent>();
            root.AddComponent<UIPathComponent>();
            // ResourcesComponent resourcesComponent = root.AddComponent<ResourcesComponent>();
            // root.AddComponent<ResourcesLoaderComponent>();
            root.AddComponent<PlayerComponent>();
            root.AddComponent<CurrentScenesComponent>();
            
            // await resourcesComponent.LoadBundleAsync("unit.unity3d");
            
            // 根据配置修改掉Main Fiber的SceneType
            SceneType sceneType = EnumHelper.FromString<SceneType>(GlobalConfig.Instance.AppType.ToString());
            root.SceneType = sceneType;
            root.AddComponent<ResComponent>();
            // await root.GetComponent<UIComponent>().ShowWindowAsync(WindowID.WindowID_Login);
            await EventSystem.Instance.PublishAsync(root, new AppStartInitFinish());
        }
    }
}