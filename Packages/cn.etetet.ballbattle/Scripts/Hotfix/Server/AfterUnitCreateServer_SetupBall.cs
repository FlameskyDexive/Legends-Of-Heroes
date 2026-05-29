namespace ET.Server
{
    // ball-battle 接线:单位在专属地图(BallMapName)创建时,懒装配竞技场 + 把玩家装配成球。
    // 只对 ball-battle 地图生效(组件隔离),普通 Map 玩法完全不受影响。
    [Event(SceneType.Map)]
    public class AfterUnitCreateServer_SetupBall : AEvent<Scene, AfterUnitCreateServer>
    {
        protected override async ETTask Run(Scene scene, AfterUnitCreateServer a)
        {
            Unit unit = a.Unit;
            if (unit == null || unit.IsDisposed)
            {
                await ETTask.CompletedTask;
                return;
            }

            // 仅 ball-battle 专属地图
            if (scene.Name.GetSceneConfigName() != BallDefine.BallMapName)
            {
                await ETTask.CompletedTask;
                return;
            }

            // 懒装配竞技场(碰撞世界 + 食物刷新器);BallArenaComponent.Awake 会建好碰撞世界
            if (scene.GetComponent<BallArenaComponent>() == null)
            {
                scene.AddComponent<BallArenaComponent>();
            }

            // 玩家球装配(食物球由 Arena 食物刷新器自行 SetupBall,这里只管玩家)
            if (unit.UnitType == UnitType.Player && unit.GetComponent<BallComponent>() == null)
            {
                unit.SetupBall(EBallType.Player);
            }

            await ETTask.CompletedTask;
        }
    }
}
