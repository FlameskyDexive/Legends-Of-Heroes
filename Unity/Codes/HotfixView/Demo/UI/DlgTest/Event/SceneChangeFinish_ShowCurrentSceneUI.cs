namespace ET
{
    
    public class SceneChangeFinish_ShowCurrentSceneUI: AEvent<EventType.SceneChangeFinish>
    {
        protected override async ETTask Run(EventType.SceneChangeFinish args)
        {
            args.ZoneScene.GetComponent<UIComponent>().HideWindow(WindowID.WindowID_Lobby);
            await args.ZoneScene.CurrentScene().GetComponent<UIComponent>().ShowWindowAsync(WindowID.WindowID_Test);
        }
    }
    
}