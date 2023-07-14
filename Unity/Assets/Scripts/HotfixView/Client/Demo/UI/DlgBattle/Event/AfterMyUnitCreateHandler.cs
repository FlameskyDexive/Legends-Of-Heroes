using ET.EventType;

namespace ET.Client
{
    [Event(SceneType.Current)]
    public class AfterMyUnitCreateHandler : AEvent<Scene, EventType.AfterMyUnitCreate>
    {
        protected override async ETTask Run(Scene scene, AfterMyUnitCreate a)
        {
            
            scene.GetComponent<UIComponent>()?.GetDlgLogic<DlgBattle>()?.RefreshSkillView();
            await ETTask.CompletedTask;
        }
        
    }
}