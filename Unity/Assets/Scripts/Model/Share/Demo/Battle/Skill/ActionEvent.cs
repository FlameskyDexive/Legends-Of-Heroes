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
    public class ActionEvent : Entity,IAwake<int, int, EActionEventSourceType>,IDestroy,ITransfer
    {
        [BsonIgnore]
        public Unit OwnerUnit
        {
            get
            {
                switch (this.SourceType)
                {
                    case EActionEventSourceType.Skill:
                        return this.GetParent<SkillTimelineComponent>().Unit;
                    case EActionEventSourceType.Buff:
                        return this.GetParent<SkillTimelineComponent>().Unit;
                    case EActionEventSourceType.Bullet:
                        return this.GetParent<SkillTimelineComponent>().Unit;
                }
                
                return this.GetParent<SkillTimelineComponent>().Unit;
            }
        }

        public Skill OwnerSkill => this.GetParent<SkillTimelineComponent>().GetParent<Skill>();

        public SkillConfig SkillConfig => this.OwnerSkill.SkillConfig;

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

        public int ConfigId;

        public ActionEventConfig ActionEventConfig
        {
            get
            {
                return ActionEventConfigCategory.Instance.Get(this.ConfigId);
            }
        }

        

    }
}