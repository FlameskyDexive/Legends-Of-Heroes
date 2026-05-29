using System.Numerics;

namespace ET
{
    // 把一个 Unit 装配成"球":加类型标记 + Box2D 圆形 sensor 碰撞体 + 按 HP 算体型。
    // 双端可用(Share),玩家球/食物球/子弹球都走这里。调用前场景需已有 CollisionWorldComponent。
    public static class BallHelper
    {
        public static void SetupBall(this Unit unit, EBallType ballType)
        {
            if (unit.GetComponent<BallComponent>() == null)
            {
                unit.AddComponent<BallComponent, EBallType>(ballType);
            }

            // 先按 HP 算出 Radius(此时还没碰撞体,Radius watcher 会 no-op)
            unit.RecalcFromHp();

            float radius = unit.GetRadius();
            if (radius < 0.01f)
            {
                radius = 0.5f; // 兜底,避免 0 半径
            }

            CollisionComponent collision = unit.GetComponent<CollisionComponent>();
            if (collision == null)
            {
                collision = unit.AddComponent<CollisionComponent>();
            }
            // 圆形 sensor:只检测重叠,不做物理反弹。userData=unit 供接触时反查实体。
            collision.AddCollider(EColliderType.Circle, new Vector2(radius, 0), Vector2.Zero, true, unit);
        }
    }
}
