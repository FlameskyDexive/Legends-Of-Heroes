namespace ET
{
    public class AfterCreateZoneScene_AddComponent: AEvent<EventType.AfterCreateZoneScene>
    {
        protected override async ETTask Run(EventType.AfterCreateZoneScene args)
        {
            Scene zoneScene = args.ZoneScene;
            zoneScene.AddComponent<UIComponent>();
            zoneScene.AddComponent<RedDotComponent>();
            UIComponent.Instance.ShowWindow(WindowID.WindowID_RedDot);
            await ETTask.CompletedTask;
        }
    }
}