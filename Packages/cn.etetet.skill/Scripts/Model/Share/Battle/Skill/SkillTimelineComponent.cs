using System.Collections.Generic;

namespace ET
{

    [ComponentOf(typeof (Skill))]
    public class SkillTimelineComponent: Entity, IAwake<int, int>, ITransfer, IUpdate
    {
        public Unit Unit => this.GetParent<Skill>().Unit;
        public SkillConfig Skillconfig;
        /// <summary>
        /// 技能开始释放时的时间戳
        /// </summary>
        public long StartSpellTime;
    }
}