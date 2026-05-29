namespace ET
{
    // 服务端:Radius 变化时同步更新 Box2D 碰撞圆半径,保证吞噬/命中判定与体型一致。
    [NumericWatcher(SceneType.Map, NumericType.Radius)]
    public class NumericWatcher_Radius_UpdateCollision : INumericWatcher
    {
        public void Run(Unit unit, NumbericChange args)
        {
            unit.GetComponent<CollisionComponent>()?.SetBodyCircleRadius(args.New / 1000f);
        }
    }
}
