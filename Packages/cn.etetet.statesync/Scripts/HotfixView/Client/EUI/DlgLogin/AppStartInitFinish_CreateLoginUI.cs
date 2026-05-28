namespace ET.Client
{
    [Event(SceneType.Client)]
    public class AppStartInitFinish_CreateLoginUI : AEvent<Scene, AppStartInitFinish>
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

            // 热更检测：进入登录前先跑增量补丁下载（HostPlayMode 才会有补丁；编辑器/离线模式为空操作）
            EntityRef<Scene> rootRef = root;
            await HotUpdateHelper.HotUpdateAsync(root);
            root = rootRef;

            await root.GetComponent<UIComponent>().ShowWindowAsync(WindowID.WindowID_Login);
        }
    }
}
