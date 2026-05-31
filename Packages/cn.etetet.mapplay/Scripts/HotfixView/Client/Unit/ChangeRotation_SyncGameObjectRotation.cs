using UnityEngine;

namespace ET.Client
{
    [Event(SceneType.Current)]
    public class ChangeRotation_SyncGameObjectRotation: AEvent<Scene, ChangeRotation>
    {
        protected override async ETTask Run(Scene scene, ChangeRotation args)
        {
            Unit unit = args.Unit;
            GameObjectComponent gameObjectComponent = unit.GetComponent<GameObjectComponent>();
            if (gameObjectComponent == null)
            {
                return;
            }

            // 有 RootDir 子节点(递归找)就只转它、否则转根节点。与 ChangePosition 共用同一逻辑,避免两处不一致。
            GameObjectPosHelper.SyncRotation(gameObjectComponent.GameObject, unit.Rotation);
            await ETTask.CompletedTask;
        }
    }
}
