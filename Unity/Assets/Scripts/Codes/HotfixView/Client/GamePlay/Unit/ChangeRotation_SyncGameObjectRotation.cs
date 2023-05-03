using UnityEngine;

namespace ET.Client
{
    [Event(SceneType.Current)]
    public class ChangeRotation_SyncGameObjectRotation: AEvent<Scene, EventType.ChangeRotation>
    {
        protected override async ETTask Run(Scene scene, EventType.ChangeRotation args)
        {
            Unit unit = args.Unit;
            unit.GetComponent<GameObjectComponent>()?.SyncUnitTransform();
            
            await ETTask.CompletedTask;
        }
    }
}
