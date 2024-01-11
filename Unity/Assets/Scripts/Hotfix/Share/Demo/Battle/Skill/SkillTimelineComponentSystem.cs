using System;
using System.Collections.Generic;
using ET.EventType;

namespace ET
{
    [FriendOf(typeof(SkillComponent))]
    [EntitySystemOf(typeof(SkillTimelineComponent))]
    [FriendOf(typeof(SkillTimelineComponent))]
    [FriendOf(typeof(SkillEvent))]
    public static partial class SkillTimelineComponentSystem
    {
        
        [EntitySystem]
        private static void Awake(this SkillTimelineComponent self, int skillId, int skillLevel)
        {
            //当前测试，一个事件一个字段，可以自己换成二维数组一个字段存多条事件数据
            self.Skillconfig = SkillConfigCategory.Instance.Get(skillId, skillLevel);
            
        }
        
        /// <summary>
        /// 固定帧驱动
        /// </summary>
        /// <param name="self"></param>
        [EntitySystem]
        public static void FixedUpdate(this SkillTimelineComponent self)
        {
            using (ListComponent<long> list = ListComponent<long>.Create())
            {
                long timeNow = TimeInfo.Instance.ServerNow();
                foreach ((long key, Entity value) in self.Children)
                {
                    SkillEvent skillEvent = (SkillEvent)value;

                    if (timeNow > skillEvent.EventTriggerTime)
                    {
                        SkillWatcherComponent.Instance.Run(skillEvent, new SkillEventType(){skillEventType = skillEvent.SkillEventType, owner = skillEvent.Unit});
                        list.Add(key);
                    }
                }

                foreach (long id in list)
                {
                    self.Remove(id);
                }
            }
        }
        
        public static void StartPlay(this SkillTimelineComponent self)
        {
            self.StartSpellTime = TimeInfo.Instance.ServerNow();
            self.InitEvents();
        }
        
        public static void InitEvents(this SkillTimelineComponent self)
        {
            if (self.Skillconfig?.Params.Count > 0)
            {
                self.AddChild<SkillEvent, SkillConfig>(self.Skillconfig);
            }
        }

        private static void Remove(this SkillTimelineComponent self, long id)
        {
            if (!self.Children.TryGetValue(id, out Entity skillEvent))
            {
                return;
            }

            skillEvent.Dispose();
        }
    }

   
}