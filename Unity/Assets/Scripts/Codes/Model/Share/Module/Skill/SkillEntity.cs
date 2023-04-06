using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace ET
{
    [ChildOf(typeof(UnitComponent))]
    public class SkillEntity:Entity,IAwake<int>,IDestroy,ITransfer
    {
        public int SkillId;
        public int SkillLevel;
        
        [BsonIgnore]
        public SkillConfig SkillConfig => SkillConfigCategory.Instance.GetByKeys(this.SkillId, this.SkillLevel);

        public long LastSpellTime;//上次施法时间
        public long LastSpellOverTime;//上次施法完成时间


        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<string, long> Groups { get; set; }
    }
}