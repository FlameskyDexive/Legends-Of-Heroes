using Box2DSharp.Collision.Collider;
using Box2DSharp.Dynamics;
using Box2DSharp.Dynamics.Contacts;
using System.Collections.Generic;
using System.Numerics;

namespace ET
{
    [ObjectSystem]
    public class CollisionWorldAwakeSystem : AwakeSystem<CollisionWorldComponent>
    {
        protected override void Awake(CollisionWorldComponent self)
        {
            self.Awake();
        }
    }
    [ObjectSystem]
    public class CollisionWorldFixedUpdateSystem : FixedUpdateSystem<CollisionWorldComponent>
    {
        protected override void FixedUpdate(CollisionWorldComponent self)
        {
         self.FixedUpdate();   
        }
    }
    
    [ObjectSystem]
    public class CollisionWorldDestroySystem : DestroySystem<CollisionWorldComponent>
    {
        protected override void Destroy(CollisionWorldComponent self)
        {
            
        }
    }
    [FriendOf(typeof(CollisionWorldComponent))]
    public static class CollisionWorldComponentSystem
    {
        public static void Awake(this CollisionWorldComponent self)
        {
            self.World = CollisionHelper.CreateWorld(new Vector2(0, 0));
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

        /// <summary>
        /// 每帧驱动更新碰撞检测
        /// </summary>
        /// <param name="self"></param>
        public static void FixedUpdate(this CollisionWorldComponent self)
        {
            foreach (var body in self.BodyToDestroy)
            {
                self.World.DestroyBody(body);
            }
            self.BodyToDestroy.Clear();
            self.World.Step(DefineCore.FixedDeltaTime, self.VelocityIteration, self.PositionIteration);
        }



    }
}