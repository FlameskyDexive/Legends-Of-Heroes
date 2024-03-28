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
            self.ActionEventType = (EActionEventType)skillConfig.Params[1];
            self.SourceType = EActionEventSourceType.Skill;
        }
        
        [EntitySystem]
        public static void Awake(this ActionEvent self, BuffConfig buffConfig)
        {
            // self.EventData = buffConfig.Params;
            //触发时间 = 事件触发百分比 * 技能时长 + 技能触发时间
            self.EventTriggerTime = 0;
            self.SourceType = EActionEventSourceType.Buff;
            // self.ActionEventType = (EActionEventType)skillConfig.Params[1];
        }
        
        [EntitySystem]
        public static void Awake(this ActionEvent self, BulletComponent bulletComponent)
        {
            self.EventTriggerTime = 0;
            self.SourceType = EActionEventSourceType.Bullet;
            // self.ActionEventType = (EActionEventType)skillConfig.Params[1];
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