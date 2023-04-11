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
            }
        }

        
        public static bool IsInCd(this Skill self, int skillId)
        {
            return false;
        }
    }

   
}