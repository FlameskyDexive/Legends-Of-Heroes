using System;
using System.Collections.Generic;
using ET.EventType;

namespace ET
{
    [EntitySystemOf(typeof(SkillWatcherComponent))]
    [FriendOf(typeof(SkillWatcherComponent))]
    [FriendOf(typeof(SkillEvent))]
    public static partial class SkillWatcherComponentSystem
    {
        [EntitySystem]
        private static void Awake(this SkillWatcherComponent self)
        {
            SkillWatcherComponent.Instance = self;
            self.allWatchers = new Dictionary<ESkillEventType, List<ISkillWatcher>>();

            HashSet<Type> types = CodeTypes.Instance.GetTypes(typeof(SkillWatcherAttribute));
            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(SkillWatcherAttribute), false);

                foreach (object attr in attrs)
                {
                    SkillWatcherAttribute SkillWatcherAttribute = (SkillWatcherAttribute)attr;
                    ISkillWatcher obj = (ISkillWatcher)Activator.CreateInstance(type);
                    if (!self.allWatchers.ContainsKey(SkillWatcherAttribute.SkillEventType))
                    {
                        self.allWatchers.Add(SkillWatcherAttribute.SkillEventType, new List<ISkillWatcher>());
                    }
                    self.allWatchers[SkillWatcherAttribute.SkillEventType].Add(obj);
                }
            }
        }

        /// <summary>
        /// 技能时间轴执行到对应技能事件调用此分发，传入参数，去执行相对应事件逻辑
        /// </summary>
        /// <param name="self"></param>
        /// <param name="entity"></param>
        /// <param name="args"></param>
        public static void Run(this SkillWatcherComponent self, SkillEvent skillEvent, SkillEventType args)
        {
            List<ISkillWatcher> list;
            if (!self.allWatchers.TryGetValue(skillEvent.SkillEventType, out list))
            {
                return;
            }

            foreach (ISkillWatcher SkillWatcher in list)
            {
                SkillWatcher.Run(skillEvent, args);
            }
        }
    }

   
}