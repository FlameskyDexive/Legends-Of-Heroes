using System;
using System.Collections.Generic;

namespace ET
{
    [FriendOf(typeof(SkillEvent))]
    public static class SkillEventSystem
    {
        [ObjectSystem]
        public class SkillEntityAwakeSystem : AwakeSystem<SkillEvent, SkillConfig>
        {
            protected override void Awake(SkillEvent self, SkillConfig skillConfig)
            {
                self.EventData = skillConfig.Params;
                self.EventTriggerTime = skillConfig.Params[0] + TimeHelper.ServerNow();
                self.SkillEventType = (ESkillEventType)skillConfig.Params[1];
            }
        }

        
    }

   
}