using System;
using System.Collections.Generic;
using Box2DSharp.Dynamics;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
namespace ET
{
    [ComponentOf(typeof(Scene))]
    public class CollisionWorldComponent : Entity, IAwake, IFixedUpdate, IDestroy
    {
        public World World;

        public List<Body> BodyToDestroy = new List<Body>();

        public int VelocityIteration = 10;
        public int PositionIteration = 10;
    }
}