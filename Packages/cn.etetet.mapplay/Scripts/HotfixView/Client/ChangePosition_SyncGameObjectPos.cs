using Sirenix.OdinInspector;
using UnityEngine;

namespace ET.Client
{
    [Event(SceneType.Current)]
    public class ChangePosition_SyncGameObjectPos: AEvent<Scene, ChangePosition>
    {
        protected override async ETTask Run(Scene scene, ChangePosition args)
        {
            Unit unit = args.Unit;
            GameObjectComponent gameObjectComponent = unit.GetComponent<GameObjectComponent>();
            if (gameObjectComponent == null)
            {
                return;
            }

            Transform transform = gameObjectComponent.Transform;

            transform.position = unit.Position;
            // 朝向:有 RootDir 子节点就只转它(头像等不动)、否则转根。与 ChangeRotation 共用 SyncRotation 保持一致。
            GameObjectPosHelper.SyncRotation(gameObjectComponent.GameObject, unit.Rotation);

            // 贴地
            GameObjectPosHelper.OnTerrain(transform);

            // 摄像机跟随
            CinemachineComponent cinemachineComponent = unit.GetComponent<CinemachineComponent>();
            if (cinemachineComponent != null)
            {
                cinemachineComponent.Follow.position = cinemachineComponent.Head.position;
            }
            
            await ETTask.CompletedTask;
        }
    }
}