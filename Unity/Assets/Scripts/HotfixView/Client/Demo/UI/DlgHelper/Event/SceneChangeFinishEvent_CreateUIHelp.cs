namespace ET.Client
{
    [Event(SceneType.Current)]
    public class SceneChangeFinishEvent_CreateUIHelp : AEvent<Scene, SceneChangeFinish>
    {
        protected override async ETTask Run(Scene scene, SceneChangeFinish args)
        {
             scene.GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_Helper);
             await ETTask.CompletedTask;
        }
    }
}