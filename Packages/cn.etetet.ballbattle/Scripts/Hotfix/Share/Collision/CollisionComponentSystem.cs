using System.Numerics;
using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;
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

        // 添加碰撞器。球用 Circle + isSensor=true(只检测重叠,不做物理碰撞反弹)。
        // 世界坐标按 X/Z 平面映射到 Box2D 的 X/Y。
        public static void AddCollider(this CollisionComponent self, EColliderType colliderType, Vector2 vec2, Vector2 offset, bool isSensor, object userData, float angle = 0)
        {
            CollisionWorldComponent world = self.WorldComponent;
            if (world == null)
            {
                return;
            }
            self.Body = world.CreateDynamicBody();
            switch (colliderType)
            {
                case EColliderType.Circle:
                    self.Body.CreateCircleFixture(vec2.X, offset, isSensor, userData ?? self.Unit);
                    break;
                case EColliderType.Box:
                    self.Body.CreateBoxFixture(vec2.X, vec2.Y, offset, angle, isSensor, userData ?? self.Unit);
                    break;
            }
        }

        // 每帧把 Unit 的位置/朝向同步到 Box2D Body(Unit 位置权威, Box2D 仅检测)
        [EntitySystem]
        public static void Update(this CollisionComponent self)
        {
            self.SyncColliderBody();
        }

        // 血量/体型变化时动态更新碰撞圆半径
        public static void SetBodyCircleRadius(this CollisionComponent self, float radius)
        {
            if (self.Body != null && self.Body.FixtureList.Count > 0)
            {
                if (self.Body.FixtureList[0].Shape is CircleShape circle)
                {
                    circle.Radius = radius;
                }
            }
        }

        public static void SyncColliderBody(this CollisionComponent self)
        {
            if (self.Body == null)
            {
                return;
            }
            Unit unit = self.GetParent<Unit>();
            float3 forward = unit.Forward;
            float angle = math.atan2(forward.x, forward.z); // Y 轴朝向映射到 Box2D 平面角(圆形无关紧要)
            self.Body.SetTransform(new Vector2(unit.Position.x, unit.Position.z), angle);
        }

        public static void SetColliderBodyState(this CollisionComponent self, bool state)
        {
            if (self.Body != null)
            {
                self.Body.IsEnabled = state;
            }
        }

        [EntitySystem]
        public static void Destroy(this CollisionComponent self)
        {
            CollisionWorldComponent world = self.WorldComponent;
            if (self.Body != null && world != null)
            {
                world.AddBodyTobeDestroyed(self.Body);
            }
            self.Body = null;
            self.WorldComponent = default;
        }
    }
}
