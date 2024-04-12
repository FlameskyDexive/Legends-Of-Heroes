using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
namespace ET
{
    namespace EventType
    {
        public struct ActionEventData
        {
            public EActionEventType actionEventType;
            public Unit owner;
            public Unit target;

        }
    }
    
    [ComponentOf(typeof(Unit))]
    public class SkillComponent:Entity,IAwake,IAwake<List<int>>,IDestroy,ITransfer
    {
        [BsonIgnore]
        public Unit Unit => this.GetParent<Unit>();
        
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<int, long> IdSkillMap = new Dictionary<int, long>();

        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<ESkillAbstractType, List<long>> AbstractTypeSkills = new Dictionary<ESkillAbstractType, List<long>>();


        public Dictionary<int, long> SkillDic = new Dictionary<int, long>();

    }
}