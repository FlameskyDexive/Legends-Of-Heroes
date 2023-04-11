using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace ET
{

    [ComponentOf(typeof (Skill))]
    public class SkillTimelineComponent: Entity, IAwake<int, int>, ITransfer, IFixedUpdate
    {
        public SkillConfig Skillconfig;
        /// <summary>
        /// 技能开始释放时的时间戳
        /// </summary>
        public long StartSpellTime;
    }
}