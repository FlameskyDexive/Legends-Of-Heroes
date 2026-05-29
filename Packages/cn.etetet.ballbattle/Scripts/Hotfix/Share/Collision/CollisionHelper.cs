using System.Numerics;
using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;

namespace ET
{
    // Box2D fixture / world 创建辅助(从 Ori 迁移)。
    public static class CollisionHelper
    {
        public static void CreateBoxFixture(this Body self, float hx, float hy, Vector2 offset, float angle, bool isSensor, object userData)
        {
            PolygonShape boxShape = new PolygonShape();
            boxShape.SetAsBox(hx, hy, offset, angle);
            FixtureDef fixtureDef = new FixtureDef
            {
                IsSensor = isSensor,
                Shape = boxShape,
                UserData = userData,
            };
            self.CreateFixture(fixtureDef);
        }

        public static void CreateCircleFixture(this Body self, float radius, Vector2 offset, bool isSensor, object userData)
        {
            CircleShape circleShape = new CircleShape
            {
                Radius = radius,
                Position = offset,
            };
            FixtureDef fixtureDef = new FixtureDef
            {
                IsSensor = isSensor,
                Shape = circleShape,
                UserData = userData,
            };
            self.CreateFixture(fixtureDef);
        }

        public static Box2DSharp.Dynamics.World CreateWorld(Vector2 gravity)
        {
            return new Box2DSharp.Dynamics.World(gravity);
        }
    }
}
