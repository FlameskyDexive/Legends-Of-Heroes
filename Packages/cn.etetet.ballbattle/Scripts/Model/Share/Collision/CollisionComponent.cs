using Box2DSharp.Dynamics;

namespace ET
{
    // 单个 Unit 的碰撞体,持有一个 Box2D Body(球用 Circle sensor)。挂 Unit。
    // 原 Ori IFixedUpdate,本项目改 IUpdate。
    [ComponentOf(typeof(Unit))]
    public class CollisionComponent : Entity, IAwake, IUpdate, IDestroy, ITransfer
    {
        public Unit Unit => this.GetParent<Unit>();

        public EntityRef<CollisionWorldComponent> WorldComponent { get; set; }

        public Body Body;
    }
}
