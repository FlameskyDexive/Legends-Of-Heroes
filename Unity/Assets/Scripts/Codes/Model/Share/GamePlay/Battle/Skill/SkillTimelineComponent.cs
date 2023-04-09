using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace ET
{

    [ComponentOf(typeof (Unit))]
    public class SkillTimeLineComponent: Entity, IAwake, ITransfer, IUpdate
    {
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<int, long> SkillEventDict = new Dictionary<int, long>();

    }
}