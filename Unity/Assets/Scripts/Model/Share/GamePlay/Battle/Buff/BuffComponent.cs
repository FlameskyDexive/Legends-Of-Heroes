using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace ET
{


    [ComponentOf(typeof (Unit))]
    public class BuffComponent: Entity, IAwake, ITransfer, IDestroy
    {
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<int, EntityRef<Buff>> BuffDic = new Dictionary<int, EntityRef<Buff>>();

    }
}