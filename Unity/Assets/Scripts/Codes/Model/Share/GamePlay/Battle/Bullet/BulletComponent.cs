using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
namespace ET
{
    [ComponentOf(typeof(Unit))]
    public class BulletComponent:Entity,IAwake, IFixedUpdate,IDestroy
    {
        [BsonIgnore]
        public Unit Unit => this.GetParent<Unit>();

        public long Timer;

    }
}