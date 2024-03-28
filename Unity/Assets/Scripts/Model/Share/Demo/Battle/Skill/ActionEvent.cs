using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace ET
{
    /// <summary>
    /// 行为事件实体，
    /// 由技能触发的时候作为技能事件时间轴组件的child
    /// 由buff触发的时候作为buff的child
    /// </summary>
    [ChildOf()]
    public class ActionEvent : Entity,IAwake<SkillConfig>,IAwake<BuffConfig>,IAwake<BulletComponent>,IDestroy,ITransfer
    {
        [BsonIgnore]
        public Unit Unit => this.GetParent<SkillTimelineComponent>().Unit;

        public Skill Skill => this.GetParent<SkillTimelineComponent>().GetParent<Skill>();

        public SkillConfig SkillConfig => this.Skill.SkillConfig;

        public EActionEventSourceType SourceType;

        public EActionEventType ActionEventType;
        /// <summary>
        /// 行为事件出发的时间戳，拿当前事件对比，每次释放技能会重置时间戳
        /// buff等触发时间未0
        /// </summary>
        public long EventTriggerTime;
        /// <summary>
        /// 当前事件是否已经触发
        /// </summary>
        public bool HasTrigger;

        public List<int> EventData;
        

    }
}