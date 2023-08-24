using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace ET
{
   
    namespace EventType
    {
        public struct SkillEventType
        {
			public ESkillEventType skillEventType;
            public Unit owner;
            public Unit target;
            //技能数据来源放在此处，如果有技能编辑器，对接编辑器数据；如果是表格配置技能数据则来源表格。

        }
    }

    [ComponentOf(typeof (BattleUnitComponent))]
    public class SkillComponent: Entity, IAwake, ITransfer
    {
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<int, long> SkillDic = new Dictionary<int, long>();

    }
}