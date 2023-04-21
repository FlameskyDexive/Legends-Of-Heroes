using System.Collections;
using System.Collections.Generic;
using Box2DSharp.Collision.Collider;
using Box2DSharp.Dynamics;
using Box2DSharp.Dynamics.Contacts;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace ET
{
    public class Box2dManager : MonoBehaviour, IContactListener
    {

        public World world;
        // Start is called before the first frame update
        void Awake()
        {
            world = new World(Vector2.Zero);
            world.CreateBody(new BodyDef() { BodyType = BodyType.DynamicBody, AllowSleep = false });
            world.SetContactListener(this);
        }

        // Update is called once per frame
        void Update()
        {
            world.Step(DefineCore.FixedDeltaTime, 10, 10);
        }

        public Body CreateDynamicBody()
        {
            return world.CreateBody(new BodyDef() { BodyType = BodyType.DynamicBody, AllowSleep = false });
        }


        public void BeginContact(Contact contact)
        {
            Collider col1 = contact.FixtureA.UserData as Collider;
            Collider col2 = contact.FixtureB.UserData as Collider;

            Log.Info($"begin contact:{col1?.gameObject.name}, {col2?.gameObject.name}");
        }

        public void EndContact(Contact contact)
        {
            Collider col1 = contact.FixtureA.UserData as Collider;
            Collider col2 = contact.FixtureB.UserData as Collider;

            Log.Info($"end contact:{col1?.gameObject.name}, {col2?.gameObject.name}");
        }

        public void PreSolve(Contact contact, in Manifold oldManifold)
        {

        }

        public void PostSolve(Contact contact, in ContactImpulse impulse)
        {

        }
    }
}
