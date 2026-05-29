using System.Numerics;
using Box2DSharp.Dynamics;

namespace ET
{
    [EntitySystemOf(typeof(CollisionWorldComponent))]
    [FriendOf(typeof(CollisionWorldComponent))]
    public static partial class CollisionWorldComponentSystem
    {
        [EntitySystem]
        public static void Awake(this CollisionWorldComponent self)
        {
            self.World = CollisionHelper.CreateWorld(new Vector2(0, 0)); // 顶视 2D,无重力
            self.World.SetContactListener(self.GetParent<Scene>().GetComponent<CollisionListenerComponent>());
        }

        public static Body CreateDynamicBody(this CollisionWorldComponent self)
        {
            return self.World.CreateBody(new BodyDef() { BodyType = BodyType.DynamicBody, AllowSleep = false });
        }

        public static Body CreateStaticBody(this CollisionWorldComponent self)
        {
            return self.World.CreateBody(new BodyDef() { BodyType = BodyType.StaticBody });
        }

        public static void AddBodyTobeDestroyed(this CollisionWorldComponent self, Body body)
        {
            self.BodyToDestroy.Add(body);
        }

        // 每帧驱动碰撞检测(原 Ori IFixedUpdate, 本项目用 IUpdate + 固定 dt)
        [EntitySystem]
        public static void Update(this CollisionWorldComponent self)
        {
            foreach (Body body in self.BodyToDestroy)
            {
                self.World.DestroyBody(body);
            }
            self.BodyToDestroy.Clear();
            self.World.Step(BallDefine.FixedDeltaTime, self.VelocityIteration, self.PositionIteration);
        }

        [EntitySystem]
        public static void Destroy(this CollisionWorldComponent self)
        {
            self.World = null;
            self.BodyToDestroy.Clear();
        }
    }
}
