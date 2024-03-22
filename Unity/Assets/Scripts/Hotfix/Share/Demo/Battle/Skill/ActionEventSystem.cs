using System;
using System.Collections.Generic;

namespace ET
{
    [EntitySystemOf(typeof(ActionEvent))]
    [FriendOf(typeof(ActionEvent))]
    public static partial class ActionEventSystem
    {
        
        [EntitySystem]
        public static void Awake(this ActionEvent self, SkillConfig skillConfig)
        {
            self.EventData = skillConfig.Params;
            //触发时间 = 事件触发百分比 * 技能时长 + 技能触发时间
            self.EventTriggerTime = skillConfig.Params[0] * self.EventData[2] / 100 + TimeInfo.Instance.ServerNow();
            self.actionEventType = (EActionEventType)skillConfig.Params[1];
        }

        public static void Transfer(this ActionEvent self)
        {
            
        }
        [EntitySystem]
        public static void Destroy(this ActionEvent self)
        {
            
        }
    }
}