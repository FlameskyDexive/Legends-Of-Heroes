using System;
using System.Collections.Generic;
using ET.EventType;

namespace ET
{
    [FriendOf(typeof(SkillComponent))]
    [FriendOf(typeof(SkillTimelineComponent))]
    [FriendOf(typeof(SkillEvent))]
    public static class SkillTimelineComponentSystem
    {
        [ObjectSystem]
        public class SkillTimelineComponentAwakeSystem : AwakeSystem<SkillTimelineComponent, int, int>
        {
            protected override void Awake(SkillTimelineComponent self, int skillId, int skillLevel)
            {
                self.Awake(skillId, skillLevel);
            }
        }
        [ObjectSystem]
        public class SkillTimelineComponentFixedUpdateSystem : FixedUpdateSystem<SkillTimelineComponent>
        {
            protected override void FixedUpdate(SkillTimelineComponent self)
            {
                self.FixedUpdate();
            }
        }

	

        private static void Awake(this SkillTimelineComponent self, int skillId, int skillLevel)
        {
            //当前测试，一个事件一个字段，可以自己换成二维数组一个字段存多条事件数据
            self.Skillconfig = SkillConfigCategory.Instance.GetByKeys(skillId, skillLevel);
            
        }
        
        /// <summary>
        /// 固定帧驱动
        /// </summary>
        /// <param name="self"></param>
        public static void FixedUpdate(this SkillTimelineComponent self)
        {
            using (ListComponent<long> list = ListComponent<long>.Create())
            {
                long timeNow = TimeHelper.ServerNow();
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
            self.StartSpellTime = TimeHelper.ServerNow();
            self.InitEvents();
        }
        
        public static void InitEvents(this SkillTimelineComponent self)
        {
            if (self.Skillconfig?.Params.Length > 0)
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