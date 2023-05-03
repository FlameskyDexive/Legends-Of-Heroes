namespace ET.Client
{
    [Event(SceneType.Current)]
    public class AfterCreateCurrentScene_AddComponent: AEvent<Scene, EventType.AfterCreateCurrentScene>
    {
        protected override async ETTask Run(Scene scene, EventType.AfterCreateCurrentScene args)
        {
            scene.AddComponent<UIComponent>();
            // scene.AddComponent<ResourcesLoaderComponent>();


            scene.AddComponent<OperaComponent>();
            scene.AddComponent<CameraComponent>();

            scene.GetComponent<UIComponent>()?.ShowWindow(WindowID.WindowID_Battle);

            await ETTask.CompletedTask;
        }
    }
}