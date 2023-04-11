using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace ET
{

    [ComponentOf(typeof (Skill))]
    public class SkillTimeLineComponent: Entity, IAwake<int, int>, ITransfer, IFixedUpdate
    {
        

    }
}