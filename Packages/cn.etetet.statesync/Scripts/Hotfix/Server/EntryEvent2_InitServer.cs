namespace ET.Server
{
    [Event(SceneType.StateSync)]
    public class EntryEvent2_InitServer: AEvent<Scene, EntryEvent2>
    {
        protected override async ETTask Run(Scene root, EntryEvent2 args)
        {
            LogMsg.Instance.AddIgnore(typeof(ServiceHeartbeatRequest));
            LogMsg.Instance.AddIgnore(typeof(ServiceHeartbeatResponse));
            
            Fiber fiber = root.Fiber;
            EntityRef<Scene> rootRef = root;
            
            int process = Options.Instance.Process;
            StartProcessConfig startProcessConfig = fiber.GetSingleton<StartProcessConfigCategory>().Get(process);
            
            World.Instance.AddSingleton<AddressSingleton>();
            // 先看环境变量是否有地址传过来，如果没有，则使用StartProcessConfig的地址跟端口
            AddressHelper.SetInnerIPInnerPortOuterIP(fiber, startProcessConfig);
            
            // 因为bind的地址有可能是0.0.0.0:0,NetInner创建完成会设置具体的地址到AddressSingleton中
            await fiber.CreateFiber(SchedulerType.ThreadPool, 0, IdGenerater.Instance.GenerateId(), SceneType.NetInner,
                $"NetInner@{process}@{Options.Instance.ReplicaIndex}");

            await fiber.CreateFiber(SchedulerType.ThreadPool, 0, IdGenerater.Instance.GenerateId(), SceneType.ServiceDiscoveryAgent,
                $"ServiceDiscoveryAgent@{process}@{Options.Instance.ReplicaIndex}");

            if (startProcessConfig != null)
            {
                // 根据配置创建纤程
                var scenes = fiber.GetSingleton<StartSceneConfigCategory>().GetByProcess(process);

                foreach (StartSceneConfig startConfig in scenes)
                {
                    int sceneType = SceneTypeSingleton.Instance.GetSceneType(startConfig.SceneType);
                    if (sceneType == SceneType.ServiceDiscovery)
                    {
                        await fiber.CreateFiber(SchedulerType.ThreadPool, 0, startConfig.Id, sceneType,
                            $"{startConfig.Name}@{process}@{Options.Instance.ReplicaIndex}");
                    }
                    else
                    {
                        await fiber.CreateFiber(SchedulerType.ThreadPool, startConfig.Zone, startConfig.Id, sceneType,
                            $"{startConfig.Name}@{process}@{Options.Instance.ReplicaIndex}");
                    }
                }
            }

            root = rootRef;
            if (Options.Instance.Console == 1)
            {
                root.AddComponent<ConsoleComponent>();
                // 机器人管理器：供 GM 控制台命令 matchrobot 创建机器人玩家（模拟真实登录+进图当匹配对手）
                root.AddComponent<RobotManagerComponent>();
            }
        }
    }
}
