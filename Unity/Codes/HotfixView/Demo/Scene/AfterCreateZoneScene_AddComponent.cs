namespace ET
{
    public class AfterCreateZoneScene_AddComponent: AEvent<EventType.AfterCreateZoneScene>
    {
        protected override async ETTask Run(EventType.AfterCreateZoneScene args)
        {
            Scene zoneScene = args.ZoneScene;
            zoneScene.AddComponent<UIEventComponent>();
            zoneScene.AddComponent<UIPathComponent>();
            zoneScene.AddComponent<UIComponent>();
            zoneScene.AddComponent<RedDotComponent>();
            zoneScene.AddComponent<ResourcesLoaderComponent>();
            zoneScene.GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_Login);
            await ETTask.CompletedTask;
        }
    }
}