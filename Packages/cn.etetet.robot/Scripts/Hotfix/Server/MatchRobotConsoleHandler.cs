using System;
using CommandLine;

namespace ET.Server
{
    // GM：创建机器人玩家模拟匹配对手。
    // 用法（服务端控制台）：
    //   MatchRobot              先进入 MatchRobot 模式
    //   --Num 1                 创建 1 个机器人（默认 1）
    // 机器人走真实玩家流程：登录 -> 进共享地图(Map2)，从而成为正在匹配/已进图玩家的对手，防止匹配不到人。
    [ConsoleHandler(ConsoleMode.MatchRobot)]
    public class MatchRobotConsoleHandler: IConsoleHandler
    {
        public async ETTask Run(Fiber fiber, ModeContex contex, string content)
        {
            EntityRef<ModeContex> contexRef = contex;
            try
            {
                switch (content)
                {
                    case ConsoleMode.MatchRobot:
                    {
                        Log.Console("MatchRobot mode: 输入 --Num <数量> 创建机器人匹配对手");
                        break;
                    }
                    default:
                    {
                        CreateRobotArgs options = null;
                        Parser.Default.ParseArguments<CreateRobotArgs>(content.Split(' '))
                                .WithNotParsed(error => throw new Exception("MatchRobotArgs error!"))
                                .WithParsed(o => { options = o; });

                        RobotManagerComponent robotManagerComponent = fiber.Root.GetComponent<RobotManagerComponent>();
                        if (robotManagerComponent == null)
                        {
                            Log.Console("RobotManagerComponent 未挂载，无法创建机器人（确认进程以 Console=1 启动）");
                            break;
                        }

                        EntityRef<RobotManagerComponent> robotManagerComponentRef = robotManagerComponent;

                        for (int i = 0; i < options.Num; ++i)
                        {
                            robotManagerComponent = robotManagerComponentRef;
                            // 用唯一账号，避免与已存在机器人重名
                            string account = $"MatchRobot_{IdGenerater.Instance.GenerateId()}";
                            await robotManagerComponent.NewRobot(options.SchedulerType, account);
                            Log.Console($"Create match robot: {account}");
                        }
                        break;
                    }
                }
            }
            finally
            {
                contex = contexRef;
                contex.Parent.RemoveComponent<ModeContex>();
            }
        }
    }
}
