namespace ET.Server
{
    // 客户端匹配超时兜底：请求服务端创建一个机器人玩家当对手。
    // 在 Gate 上懒挂载 RobotManagerComponent 并创建机器人（机器人走真实流程：登录 -> 进共享地图 Map2）。
    // 机器人创建是后台进行（Coroutine），不阻塞本次响应，客户端可立刻进图。
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_RequestMatchRobotHandler: MessageSessionHandler<C2G_RequestMatchRobot, G2C_RequestMatchRobot>
    {
        protected override async ETTask Run(Session session, C2G_RequestMatchRobot request, G2C_RequestMatchRobot response)
        {
            Scene root = session.Root();
            RobotManagerComponent robotManager = root.GetComponent<RobotManagerComponent>();
            if (robotManager == null)
            {
                robotManager = root.AddComponent<RobotManagerComponent>();
            }

            string account = $"MatchRobot_{IdGenerater.Instance.GenerateId()}";
            robotManager.NewRobot(SchedulerType.Parent, account).Coroutine();

            await ETTask.CompletedTask;
        }
    }
}
