using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace ET
{
    public static class CollisionHelper
    {
        public static void CreateBoxFixture(this Body self, float hx, float hy, Vector2 offset, float angle, bool isSensor, object userData)
        {
            PolygonShape m_BoxShape = new PolygonShape();
            m_BoxShape.SetAsBox(hx, hy, offset, angle);
            FixtureDef fixtureDef = new FixtureDef();
            fixtureDef.IsSensor = isSensor;
            fixtureDef.Shape = m_BoxShape;
            fixtureDef.UserData = userData;
            self.CreateFixture(fixtureDef);
        }

        public static void CreateCircleFixture(this Body self, float radius, Vector2 offset, bool isSensor, object userData)
        {
            CircleShape m_CircleShape = new CircleShape();
            m_CircleShape.Radius = radius;
            m_CircleShape.Position = offset;
            FixtureDef fixtureDef = new FixtureDef();
            fixtureDef.IsSensor = isSensor;
            fixtureDef.Shape = m_CircleShape;
            fixtureDef.UserData = userData;
            self.CreateFixture(fixtureDef);
        }
        


        public static World CreateWorld(Vector2 gravity)
        {
            return new World(gravity);
        }
    }
}
