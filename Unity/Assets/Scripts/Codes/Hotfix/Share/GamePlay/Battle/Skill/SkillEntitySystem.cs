using System;
using System.Collections.Generic;

namespace ET
{
    [FriendOf(typeof(SkillEntity))]
    public static class SkillEntitySystem
    {
        [ObjectSystem]
        public class SkillEntityAwakeSystem : AwakeSystem<SkillEntity, int, int>
        {
            protected override void Awake(SkillEntity self, int skillId, int skillLevel)
            {
                self.SkillId = skillId;
                self.SkillLevel = skillLevel;
            }
        }

        
        public static bool IsInCd(this SkillEntity self, int skillId)
        {
            return false;
        }
    }

   
}