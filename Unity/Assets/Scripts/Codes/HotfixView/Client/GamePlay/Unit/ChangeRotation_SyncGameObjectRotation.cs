using UnityEngine;

namespace ET.Client
{
    [Event(SceneType.Current)]
    public class ChangeRotation_SyncGameObjectRotation: AEvent<Scene, EventType.ChangeRotation>
    {
        protected override async ETTask Run(Scene scene, EventType.ChangeRotation args)
        {
            Unit unit = args.Unit;
            GameObjectComponent gameObjectComponent = unit.GetComponent<GameObjectComponent>();
            if (gameObjectComponent == null)
            {
                return;
            }

            if (unit.Type == UnitType.Player)
            {
                Transform dirTrans = gameObjectComponent.GameObject.transform.Find("RootDir");
                if (dirTrans != null)
                {
                    dirTrans.rotation = unit.Rotation;
                    return;
                }
            }
            Transform transform = gameObjectComponent.GameObject.transform;
            transform.rotation = unit.Rotation;
            await ETTask.CompletedTask;
        }
    }
}
