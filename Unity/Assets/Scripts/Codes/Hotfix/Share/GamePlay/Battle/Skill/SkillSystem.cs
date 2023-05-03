using System;
using System.Collections.Generic;

namespace ET
{
    [FriendOf(typeof(Skill))]
    public static class SkillSystem
    {
        [ObjectSystem]
        public class SkillEntityAwakeSystem : AwakeSystem<Skill, int, int>
        {
            protected override void Awake(Skill self, int skillId, int skillLevel)
            {
                self.SkillId = skillId;
                self.SkillLevel = skillLevel;
                self.CD = 1000;
                self.AddComponent<SkillTimelineComponent,int, int>(skillId, skillLevel);
            }
        }

        public static Unit GetOwnerUnit(this Skill self)
        {
            return self.GetParent<BattleUnitComponent>().Unit;
        }
        
        public static bool IsInCd(this Skill self)
        {
            if (self.SpellStartTime + self.CD > TimeHelper.ServerNow())
                return true;
            return false;
        }

        /// <summary>
        /// 开始释放技能
        /// </summary>
        /// <param name="self"></param>
        public static void StartSpell(this Skill self)
        {
            self.SpellStartTime = TimeHelper.ServerNow();
            self.GetComponent<SkillTimelineComponent>().StartPlay();
        }
    }

   
}