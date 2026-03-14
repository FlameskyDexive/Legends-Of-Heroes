using System;
using System.Collections.Generic;

namespace ET
{
    [EntitySystemOf(typeof(ActionEvent))]
    [FriendOf(typeof(ActionEvent))]
    public static partial class ActionEventSystem
    {
        
        [EntitySystem]
        public static void Awake(this ActionEvent self, int configId, int triggerTime, EActionEventSourceType sourceType)
        {

            //EActionEventSourceType事件来源触发时间计算
            //1. 技能：技能表的触发百分比 * 技能周期 / 1000 ms
            //2. Buff：立即触发，EventTriggerTime = 0
            //3. Bullet：立即触发，EventTriggerTime = 0
            self.EventTriggerTime = triggerTime + TimeInfo.Instance.ServerNow();
            self.ConfigId = configId;
            self.ActionEventType = self.ActionEventConfig.ActionEventType;
            self.SourceType = sourceType;
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