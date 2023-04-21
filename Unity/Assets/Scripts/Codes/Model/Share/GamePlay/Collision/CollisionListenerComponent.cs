using System;
using System.Collections.Generic;
using Box2DSharp.Collision.Collider;
using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;
using Box2DSharp.Dynamics.Contacts;
using ET.EventType;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
namespace ET
{
    [EnableMethod]
    [ComponentOf(typeof(Scene))]
    public class CollisionListenerComponent : Entity,IAwake,IDestroy,IContactListener
    {

        public void BeginContact(Contact contact)
        {
            Unit unitA = (Unit)contact.FixtureA.UserData;
            Unit unitB = (Unit)contact.FixtureB.UserData;
            if (unitA.IsDisposed || unitB.IsDisposed)
            {
                return;
            }
            Log.Info($"begin contact:{unitA.Config?.Name}, {unitB.Config?.Name}");
            EventSystem.Instance.Publish(this.DomainScene(), new OnCollisionContact(){contact = contact});

        }
        public void EndContact(Contact contact)
        {
            Unit unitA = (Unit)contact.FixtureA.UserData;
            Unit unitB = (Unit)contact.FixtureB.UserData;
            if (unitA.IsDisposed || unitB.IsDisposed)
            {
                return;
            }
            Log.Info($"end contact:{unitA.Config?.Name}, {unitB.Config?.Name}");
            EventSystem.Instance.Publish(this.DomainScene(), new OnCollisionContact() { contact = contact, isEnd = true});
        }

        public void PreSolve(Contact contact, in Manifold oldManifold)
        {
        }

        public void PostSolve(Contact contact, in ContactImpulse impulse)
        {
        }
    }
}