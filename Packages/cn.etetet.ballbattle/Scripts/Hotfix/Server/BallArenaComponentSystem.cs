using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace ET.Server
{
    [EntitySystemOf(typeof(BallArenaComponent))]
    [FriendOf(typeof(BallArenaComponent))]
    public static partial class BallArenaComponentSystem
    {
        [EntitySystem]
        public static void Awake(this BallArenaComponent self)
        {
            Scene scene = self.Scene();

            // 竞技场调参(食物/地图边界)用 BallArenaComponent 的默认字段值(MaxFoodCount/FoodHp/MapMin/MapMax)。
            // (原 BallConfig 单例表读取已移除:静态/无上下文访问 .Instance 违反 ET0039;如需配置驱动,
            //  在此用 self.GetSingleton<BallConfigCategory>().Get(1) 覆盖这些字段即可。)

            // 装配 2D 碰撞世界:Listener 必须先于 World(World.Awake 会 SetContactListener)
            if (scene.GetComponent<CollisionListenerComponent>() == null)
            {
                scene.AddComponent<CollisionListenerComponent>();
            }
            if (scene.GetComponent<CollisionWorldComponent>() == null)
            {
                scene.AddComponent<CollisionWorldComponent>();
            }

            // 设置地图移动边界(取自竞技场配置,= BallArena 场景地面 Plane 的实际占地 ±50):
            // C2M_OperationHandler(玩家)与机器人 AI 都读它 clamp 移动目标,角色不会走出地面。
            MapBoundsComponent bounds = scene.GetComponent<MapBoundsComponent>() ?? scene.AddComponent<MapBoundsComponent>();
            bounds.Min = self.MapMin;
            bounds.Max = self.MapMax;

            // 启动食物刷新(每秒补足)
            self.SpawnTimerId = scene.Root().TimerComponent.NewRepeatedTimer(1000, TimerInvokeType.BallFoodSpawn, self);

            // 启动排行榜日志(每 5s 打一次当前 Top5,服务端结算观察用;客户端 UI/广播为后续)
            self.LeaderboardTimerId = scene.Root().TimerComponent.NewRepeatedTimer(5000, TimerInvokeType.BallLeaderboard, self);

            // 生成 AI 机器人对手(服务端权威驱动)
            self.SpawnRobots();
        }

        [EntitySystem]
        public static void Destroy(this BallArenaComponent self)
        {
            self.Root().TimerComponent?.Remove(ref self.SpawnTimerId);
            self.Root().TimerComponent?.Remove(ref self.LeaderboardTimerId);
        }

        // 补足场上食物数量到 MaxFoodCount
        public static void SpawnFood(this BallArenaComponent self)
        {
            if (self.FoodConfigId == 0)
            {
                return; // 未配置食物 UnitConfig,不刷(避免 Create 失败)
            }

            Scene scene = self.Scene();
            UnitComponent unitComponent = scene.GetComponent<UnitComponent>();
            if (unitComponent == null)
            {
                return;
            }

            int foodCount = 0;
            foreach (Entity child in unitComponent.Children.Values)
            {
                if (child is Unit u && u.GetBallType() == EBallType.Food)
                {
                    foodCount++;
                }
            }

            float range = self.MapMax - self.MapMin;
            int spawned = 0;
            while (foodCount < self.MaxFoodCount)
            {
                long id = IdGenerater.Instance.GenerateId();

                float x = RandomGenerator.RandFloat01() * range + self.MapMin;
                float z = RandomGenerator.RandFloat01() * range + self.MapMin;
                // 位置必须在 Create 内加 AOIEntity 前设好(传参),否则广播给客户端的是原点 → 食物全挤原点且吃不到。
                Unit food = UnitFactory.Create(scene, id, self.FoodConfigId, new float3(x, 0, z));

                // 每个食物随机直径 [FoodMinDiameter, FoodMaxDiameter](默认 0.1~0.3),反算 HP:半径=RadiusCoef*√HP → HP=(半径/RadiusCoef)²。
                // 视觉(localScale)与碰撞半径同由 Radius 驱动,故大小一致;HP 也是吃这个食物的增量(越大越值)。
                float diameter = RandomGenerator.RandFloat01() * (self.FoodMaxDiameter - self.FoodMinDiameter) + self.FoodMinDiameter;
                float foodRadius = diameter * 0.5f;
                int foodHp = (int)((foodRadius / BallDefine.RadiusCoef) * (foodRadius / BallDefine.RadiusCoef));
                if (foodHp < 1)
                {
                    foodHp = 1;
                }
                food.NumericComponent.Set(NumericType.HP, foodHp);
                food.SetupBall(EBallType.Food); // 加碰撞体 + 按 HP 算体型(静态:食物无移动/无技能,只会被吃)
                foodCount++;
                spawned++;
            }
            // 诊断:确认服务端在刷食物(若实机看不到食物,先看此日志是否打印:有=客户端视图问题;无=Arena未装配/定时器未起)
            if (spawned > 0)
            {
                Log.Info($"[球球食物] SpawnFood 刷新 +{spawned}, 场上 {foodCount}/{self.MaxFoodCount} (configId={self.FoodConfigId}, 边界 {self.MapMin}..{self.MapMax})");
            }
        }

        // 生成 RobotCount 个 AI 机器人球(复用食物球 Virtual 配置, 但 SetupBall(Player) 当玩家球 + 挂 AI)
        public static void SpawnRobots(this BallArenaComponent self)
        {
            Scene scene = self.Scene();
            UnitComponent unitComponent = scene.GetComponent<UnitComponent>();
            if (unitComponent == null)
            {
                return;
            }

            float range = self.MapMax - self.MapMin;
            for (int i = 0; i < BallDefine.RobotCount; i++)
            {
                long id = IdGenerater.Instance.GenerateId();

                float x = RandomGenerator.RandFloat01() * range + self.MapMin;
                float z = RandomGenerator.RandFloat01() * range + self.MapMin;
                // 同食物:位置在 Create 内加 AOIEntity 前设好(传参),否则机器人也会广播到原点。
                Unit robot = UnitFactory.Create(scene, id, BallDefine.RobotConfigId, new float3(x, 0, z));

                robot.NumericComponent.Set(NumericType.HP, BallDefine.RobotInitHp);
                robot.SetupBall(EBallType.Player); // 当玩家球:吃食物/小球, 也会被吃(被吃后走重生)
                robot.AddComponent<RobotBallAIComponent>();
            }
        }

        // 广播 Top5 排行榜给场景内全体真人玩家(机器人 Virtual 无 gate,自动跳过)。
        public static void BroadcastLeaderboard(this BallArenaComponent self)
        {
            Scene scene = self.Scene();
            List<Unit> top = new List<Unit>();
            BallScoreHelper.GatherTopPlayers(scene, 5, top);
            if (top.Count == 0)
            {
                return;
            }

            // 先取出排行数据(避免对每个真人重复查 Unit)
            int n = top.Count;
            long[] ids = new long[n];
            int[] hps = new int[n];
            int[] kills = new int[n];
            for (int i = 0; i < n; i++)
            {
                ids[i] = top[i].Id;
                hps[i] = top[i].NumericComponent.GetAsInt(NumericType.HP);
                kills[i] = BallScoreHelper.GetKills(top[i]);
            }
            Log.Info($"[球球排行榜] 广播 Top{n}: #1 Unit{ids[0]} HP={hps[0]} 击杀={kills[0]}");

            UnitComponent uc = scene.GetComponent<UnitComponent>();
            if (uc == null)
            {
                return;
            }
            foreach (Entity child in uc.Children.Values)
            {
                if (!(child is Unit u) || u.IsDisposed)
                {
                    continue;
                }
                // 仅真人:Player 型 + 有 gate 连接;机器人是 Virtual,且 NoticeClient 内部也会守卫
                if (!u.UnitType.IsSame(UnitType.Player) || u.GetComponent<UnitGateInfoComponent>() == null)
                {
                    continue;
                }

                M2C_BallLeaderboard msg = M2C_BallLeaderboard.Create();
                for (int i = 0; i < n; i++)
                {
                    BallRankInfo rank = BallRankInfo.Create();
                    rank.UnitId = ids[i];
                    rank.Hp = hps[i];
                    rank.Kills = kills[i];
                    msg.Ranks.Add(rank);
                }
                MapMessageHelper.NoticeClient(u, msg, NoticeType.Self);
            }
        }
    }

    // 食物刷新定时器
    [Invoke(TimerInvokeType.BallFoodSpawn)]
    public class BallFoodSpawnTimer : ATimer<BallArenaComponent>
    {
        protected override void Run(BallArenaComponent self)
        {
            try
            {
                self.SpawnFood();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }

    // 排行榜日志定时器(服务端结算)
    [Invoke(TimerInvokeType.BallLeaderboard)]
    public class BallLeaderboardTimer : ATimer<BallArenaComponent>
    {
        protected override void Run(BallArenaComponent self)
        {
            try
            {
                self.BroadcastLeaderboard();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
