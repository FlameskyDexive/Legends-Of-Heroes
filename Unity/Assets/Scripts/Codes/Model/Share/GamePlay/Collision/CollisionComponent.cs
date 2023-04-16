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
    [ComponentOf(typeof(BattleUnitComponent))]
    public class CollisionComponent : Entity,IAwake, IFixedUpdate,IDestroy,ITransfer
    {
        [BsonIgnore]
        public BattleUnitComponent BattleUnit => this.GetParent<BattleUnitComponent>();

        // public UnitConfig UnitConfig => this.BattleUnit.Unit?.Config;

        public CollisionWorldComponent WorldComponent { get; set; }
        
        public Body Body;
        

    }
}