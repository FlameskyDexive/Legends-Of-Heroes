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
        //当前暂时只处理开始碰撞，不需要处理碰撞结束，持续（激光技能需要处理持续）
        public void BeginContact(Contact contact)
        {
            EventSystem.Instance.Publish(this.DomainScene(), new OnCollisionContact(){contact = contact});
        }
        public void EndContact(Contact contact)
        {
            // EventSystem.Instance.Publish(this.DomainScene(), new OnCollisionContact() { contact = contact, isEnd = true});
        }

        public void PreSolve(Contact contact, in Manifold oldManifold)
        {
        }

        public void PostSolve(Contact contact, in ContactImpulse impulse)
        {
        }
    }
}