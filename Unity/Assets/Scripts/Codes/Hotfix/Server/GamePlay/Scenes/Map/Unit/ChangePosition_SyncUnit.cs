using Unity.Mathematics;

namespace ET.Server
{
    [Event(SceneType.Map)]
    public class ChangePosition_SyncUnit: AEvent<Scene, ET.EventType.ChangePosition>
    {
        protected override async ETTask Run(Scene scene, ET.EventType.ChangePosition args)
        {
            Unit unit = args.Unit;
            
            unit.DomainScene().GetComponent<UnitComponent>().AddSyncUnit(unit);
            await ETTask.CompletedTask;
        }
    }
}