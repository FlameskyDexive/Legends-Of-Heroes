using UnityEngine;

namespace ET.Client
{
    [Event(SceneType.Client)]
    public class AfterCreateClientScene_AddComponent: AEvent<EventType.AfterCreateClientScene>
    {
        protected override async ETTask Run(Scene scene, EventType.AfterCreateClientScene args)
        {
            scene.AddComponent<UIEventComponent>();
            scene.AddComponent<UIComponent>();
            scene.AddComponent<UIPathComponent>();
            // scene.AddComponent<ResourcesLoaderComponent>();
            scene.GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_Login);
            await ETTask.CompletedTask;
        }
    }
}