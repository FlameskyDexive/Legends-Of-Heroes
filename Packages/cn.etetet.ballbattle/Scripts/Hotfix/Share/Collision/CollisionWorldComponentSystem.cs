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

            // 🔑 World.Step 之后再裁决碰撞:Step 期间 BeginContact 只记录了接触单位对,这里(Step 外)才发事件做吞噬/移除,
            // 避免在 Step/AOI 迭代中销毁单位导致 Box2D 迭代错乱 + AOI LeaveSight 对已销毁单位建 EntityRef 崩。
            Scene scene = self.GetParent<Scene>();
            CollisionListenerComponent listener = scene.GetComponent<CollisionListenerComponent>();
            if (listener != null && listener.PendingContacts.Count > 0)
            {
                foreach ((long idA, long idB) in listener.PendingContacts)
                {
                    EventSystem.Instance.Publish(scene, new OnCollisionContact { UnitIdA = idA, UnitIdB = idB });
                }
                listener.PendingContacts.Clear();
            }
        }

        [EntitySystem]
        public static void Destroy(this CollisionWorldComponent self)
        {
            self.World = null;
            self.BodyToDestroy.Clear();
        }
    }
}
