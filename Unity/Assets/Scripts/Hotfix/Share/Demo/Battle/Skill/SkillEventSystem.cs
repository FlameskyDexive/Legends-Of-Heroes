using System;
using System.Collections.Generic;

namespace ET
{
    [EntitySystemOf(typeof(SkillEvent))]
    [FriendOf(typeof(SkillEvent))]
    public static partial class SkillEventSystem
    {
        
        [EntitySystem]
        public static void Awake(this SkillEvent self, SkillConfig skillConfig)
        {
            self.EventData = skillConfig.Params;
            //触发时间 = 事件触发百分比 * 技能时长 + 技能触发时间
            self.EventTriggerTime = skillConfig.Params[0] * self.EventData[2] / 100 + self.Fiber().TimeInfo.ServerNow();
            self.SkillEventType = (ESkillEventType)skillConfig.Params[1];
        }

        public static void Transfer(this SkillEvent self)
        {
            
        }
        [EntitySystem]
        public static void Destroy(this SkillEvent self)
        {
            
        }
    }
}