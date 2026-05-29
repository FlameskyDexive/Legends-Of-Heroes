using Unity.Mathematics;

namespace ET.Server
{
    // 玩家球死亡 -> 重生(服务端权威)。订阅 BallPlayerDie:
    // 死亡瞬间缩成最小体型并关碰撞, 延迟后在地图随机点重置 HP 复活并广播新位置。
    [Event(SceneType.Map)]
    public class BallPlayerDie_Respawn : AEvent<Scene, BallPlayerDie>
    {
        protected override async ETTask Run(Scene scene, BallPlayerDie a)
        {
            Unit dead = a.Dead;
            if (dead == null || dead.IsDisposed)
            {
                return;
            }
            await dead.RespawnBall();
        }
    }

    [FriendOf(typeof(BallArenaComponent))]
    public static class BallDeathHelper
    {
        public static async ETTask RespawnBall(this Unit unit)
        {
            Scene scene = unit.Scene();

            // 死亡瞬间: 缩成最小体型 + 关碰撞(防止复活前再次被吃; 也作为死亡表现)
            unit.NumericComponent.Set(NumericType.HP, (int)BallDefine.MinHp);
            unit.RecalcFromHp();
            unit.GetComponent<CollisionComponent>()?.SetColliderBodyState(false);

            EntityRef<Unit> unitRef = unit;
            EntityRef<Scene> sceneRef = scene;
            await scene.Root().GetComponent<TimerComponent>().WaitAsync(BallDefine.RespawnDelayMs);
            unit = unitRef;
            scene = sceneRef;
            if (unit == null || unit.IsDisposed)
            {
                return;
            }

            // 复活: 随机位置 + 重置 HP(初始体型), 经数值系统自动广播体型/速度
            BallArenaComponent arena = scene.GetComponent<BallArenaComponent>();
            float min = arena != null ? arena.MapMin : -50f;
            float max = arena != null ? arena.MapMax : 50f;
            float range = max - min;
            float x = RandomGenerator.RandFloat01() * range + min;
            float z = RandomGenerator.RandFloat01() * range + min;
            float3 pos = new float3(x, 0, z);

            unit.NumericComponent.Set(NumericType.HP, BallDefine.RespawnHp);
            unit.RecalcFromHp();
            unit.GetComponent<MoveComponent>().FlashTo(pos);
            unit.GetComponent<CollisionComponent>()?.SetColliderBodyState(true);
            unit.SendStop(0); // 广播新位置, 客户端 snap 过去
        }
    }
}
