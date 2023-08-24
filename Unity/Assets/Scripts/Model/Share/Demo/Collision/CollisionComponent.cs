using System;
using System.Collections.Generic;
using Box2DSharp.Collision.Collider;
using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;
using Box2DSharp.Dynamics.Contacts;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
namespace ET
{
    [ComponentOf(typeof(Unit))]
    public class CollisionComponent : Entity,IAwake, IFixedUpdate,IDestroy,ITransfer
    {
        [BsonIgnore]
        public Unit Unit => this.GetParent<Unit>();

        // public UnitConfig UnitConfig => this.Unit?.Config;

        public CollisionWorldComponent WorldComponent { get; set; }
        
        public Body Body;
        

    }
}