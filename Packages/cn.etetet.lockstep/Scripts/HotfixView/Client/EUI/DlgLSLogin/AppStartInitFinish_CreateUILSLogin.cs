namespace ET.Client
{
    [Event(SceneType.LockStep)]
    public class AppStartInitFinish_CreateUILSLogin : AEvent<Scene, AppStartInitFinish>
    {
        protected override async ETTask Run(Scene root, AppStartInitFinish args)
        {
            if (root.GetComponent<ResourcesLoaderComponent>() == null)
            {
                root.AddComponent<ResourcesLoaderComponent>();
            }
            if (root.GetComponent<GlobalComponent>() == null)
            {
                root.AddComponent<GlobalComponent>();
            }
            if (root.GetComponent<EUIRootComponent>() == null)
            {
                root.AddComponent<EUIRootComponent>();
            }
            if (root.GetComponent<UIEventComponent>() == null)
            {
                root.AddComponent<UIEventComponent>();
            }
            if (root.GetComponent<UIPathComponent>() == null)
            {
                root.AddComponent<UIPathComponent>();
            }
            if (root.GetComponent<UIComponent>() == null)
            {
                root.AddComponent<UIComponent>();
            }
            await root.GetComponent<UIComponent>().ShowWindowAsync(WindowID.WindowID_LSLogin);
        }
    }
}
