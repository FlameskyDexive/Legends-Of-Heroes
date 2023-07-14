using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace ET
{
    /// <summary>
    /// 技能事件实体，作为技能事件时间轴组件的child
    /// </summary>
    [ChildOf(typeof(SkillTimelineComponent))]
    public class SkillEvent : Entity,IAwake<SkillConfig>,IDestroy,ITransfer
    {
        [BsonIgnore]
        public Unit Unit => this.GetParent<SkillTimelineComponent>().Unit;

        public Skill Skill => this.GetParent<SkillTimelineComponent>().GetParent<Skill>();

        public SkillConfig SkillConfig => this.Skill.SkillConfig;


        public ESkillEventType SkillEventType;
        /// <summary>
        /// 技能事件出发的时间戳，拿当前事件对比，每次释放技能会重置时间戳
        /// </summary>
        public long EventTriggerTime;
        /// <summary>
        /// 当前事件是否已经触发
        /// </summary>
        public bool HasTrigger;

        public int[] EventData;
        

    }
}