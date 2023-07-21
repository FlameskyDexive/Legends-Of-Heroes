using System;
using System.Collections.Generic;

namespace ET
{
    [EntitySystemOf(typeof(Skill))]
    [FriendOf(typeof(Skill))]
    public static partial class SkillSystem
    {

        [EntitySystem]
        public static void Awake(this Skill self, int skillId, int skillLevel)
        {
            self.SkillId = skillId;
            self.SkillLevel = skillLevel;
            self.CD = 1000;
            self.AddComponent<SkillTimelineComponent,int, int>(skillId, skillLevel);
        }
        [EntitySystem]
        public static void Destroy(this Skill self)
        {
            
        }
        public static Unit GetOwnerUnit(this Skill self)
        {
            return self.GetParent<BattleUnitComponent>().Unit;
        }
        
        public static bool IsInCd(this Skill self)
        {
            if (self.SpellStartTime + self.CD > self.Fiber().TimeInfo.ServerNow())
                return true;
            return false;
        }

        /// <summary>
        /// 开始释放技能
        /// </summary>
        /// <param name="self"></param>
        public static void StartSpell(this Skill self)
        {
            self.SpellStartTime = self.Fiber().TimeInfo.ServerNow();
            self.GetComponent<SkillTimelineComponent>().StartPlay();
        }
    }

   
}