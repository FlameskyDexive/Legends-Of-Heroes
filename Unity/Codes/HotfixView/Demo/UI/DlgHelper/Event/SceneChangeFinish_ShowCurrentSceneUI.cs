namespace ET
{
    
    public class SceneChangeFinish_ShowCurrentSceneUI: AEventAsync<EventType.SceneChangeFinish>
    {
        protected override async ETTask Run(EventType.SceneChangeFinish args)
        {
            args.ZoneScene.GetComponent<UIComponent>().HideWindow(WindowID.WindowID_Lobby);
            await ETTask.CompletedTask;
        }
    }
    
}