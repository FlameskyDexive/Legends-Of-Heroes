using Box2DSharp.Collision.Collider;
using Box2DSharp.Collision.Shapes;
using Box2DSharp.Common;
using Box2DSharp.Dynamics;
using Box2DSharp.Dynamics.Contacts;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;

namespace ET
{
    [EntitySystemOf(typeof(CollisionComponent))]
    [FriendOf(typeof(CollisionComponent))]
    public static partial class CollisionComponentSystem
    {
        [EntitySystem]
        public static void Awake(this CollisionComponent self)
        {
            self.WorldComponent = self.Root().GetComponent<CollisionWorldComponent>();
        }
        
        /// <summary>
        /// 碰撞组件添加碰撞器
        /// </summary>
        /// <param name="self"></param>
        /// <param name="colliderType"></param>
        /// <param name="vec2"></param>
        /// <param name="offset"></param>
        /// <param name="isSensor"></param>
        /// <param name="userData"></param>
        /// <param name="angle"></param>
        public static void AddCollider(this CollisionComponent self, EColliderType colliderType, Vector2 vec2, Vector2 offset, bool isSensor, object userData, float angle = 0)
        {
            self.Fiber().Log.Info($"{self.GetParent<Unit>()?.Config?.Name} add collider:{vec2.X}");
            self.Body = self.WorldComponent.CreateDynamicBody();
            switch (colliderType)
            {
                case EColliderType.Circle:
                    self.Body.CreateCircleFixture(vec2.X, offset, isSensor, self.Unit);

                    break;
                case EColliderType.Box:
                    self.Body.CreateBoxFixture(vec2.X, vec2.Y, offset, angle, isSensor, self.Unit);

                    break;
            }
        }

        /// <summary>
        /// 每帧更新，同步位置、旋转等信息
        /// </summary>
        /// <param name="self"></param>
        [EntitySystem]
        public static void FixedUpdate(this CollisionComponent self)
        {
            self.SyncColliderBody();
        }

        /// <summary>
        /// 血量变化的时候，动态更新角色碰撞框的大小
        /// </summary>
        /// <param name="self"></param>
        /// <param name="radius"></param>
        public static void SetBodyCircleRadius(this CollisionComponent self, float radius)
        {
            if (self.Body.FixtureList.Count > 0)
            {
                Shape shape = self.Body.FixtureList[0].Shape;
                if (shape is CircleShape circle)
                {
                    circle.Radius = radius;
                }
            }
        }
        public static void SyncColliderBody(this CollisionComponent self)
        {
            Unit unit = self.GetParent<Unit>();
            self.Body.SetTransform(new Vector2(unit.Position.x, unit.Position.z), MathHelper.Angle(new float3(0, 0, 1), unit.Forward));
        }

        public static void SetColliderBodyState(this CollisionComponent self, bool state)
        {
            self.Body.IsEnabled = state;
        }

        [EntitySystem]
        public static void Destroy(this CollisionComponent self)
        {
            
        }
    }
}