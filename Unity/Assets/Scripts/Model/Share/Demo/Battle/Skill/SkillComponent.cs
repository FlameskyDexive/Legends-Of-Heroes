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
            //技能数据来源放在此处，如果有技能编辑器，对接编辑器数据；如果是表格配置技能数据则来源表格。

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