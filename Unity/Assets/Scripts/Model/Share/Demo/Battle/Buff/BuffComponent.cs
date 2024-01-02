using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace ET
{


    [ComponentOf(typeof (BattleUnitComponent))]
    public class BuffComponent: Entity, IAwake, ITransfer, IDestroy
    {
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<int, Buff> BuffDic = new Dictionary<int, Buff>();

    }
}