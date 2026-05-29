using System;
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

            // 装配 2D 碰撞世界:Listener 必须先于 World(World.Awake 会 SetContactListener)
            if (scene.GetComponent<CollisionListenerComponent>() == null)
            {
                scene.AddComponent<CollisionListenerComponent>();
            }
            if (scene.GetComponent<CollisionWorldComponent>() == null)
            {
                scene.AddComponent<CollisionWorldComponent>();
            }

            // 启动食物刷新(每秒补足)
            self.SpawnTimerId = scene.Root().TimerComponent.NewRepeatedTimer(1000, TimerInvokeType.BallFoodSpawn, self);
        }

        [EntitySystem]
        public static void Destroy(this BallArenaComponent self)
        {
            self.Root().TimerComponent?.Remove(ref self.SpawnTimerId);
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
            while (foodCount < self.MaxFoodCount)
            {
                long id = IdGenerater.Instance.GenerateId();
                Unit food = UnitFactory.Create(scene, id, self.FoodConfigId);

                float x = RandomGenerator.RandFloat01() * range + self.MapMin;
                float z = RandomGenerator.RandFloat01() * range + self.MapMin;
                food.Position = new float3(x, 0, z);

                food.NumericComponent.Set(NumericType.HP, self.FoodHp);
                food.SetupBall(EBallType.Food); // 加碰撞体 + 按 HP 算体型
                foodCount++;
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
}
