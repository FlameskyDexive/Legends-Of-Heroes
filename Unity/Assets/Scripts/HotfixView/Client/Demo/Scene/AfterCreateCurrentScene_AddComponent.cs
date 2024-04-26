namespace ET.Client
{
    [Event(SceneType.Current)]
    public class AfterCreateCurrentScene_AddComponent: AEvent<Scene, AfterCreateCurrentScene>
    {
        protected override async ETTask Run(Scene scene, AfterCreateCurrentScene args)
        {
            scene.AddComponent<UIComponent>();
            scene.AddComponent<ResourcesLoaderComponent>();
            scene.GetComponent<UIComponent>()?.ShowWindow(WindowID.WindowID_Battle);
            await ETTask.CompletedTask;
        }
    }
}