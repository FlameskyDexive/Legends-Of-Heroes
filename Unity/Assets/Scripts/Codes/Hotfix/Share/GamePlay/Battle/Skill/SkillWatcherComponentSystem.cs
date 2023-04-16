using System;
using System.Collections.Generic;
using ET.EventType;

namespace ET
{
    [FriendOf(typeof(SkillWatcherComponent))]
    [FriendOf(typeof(SkillEvent))]
    public static class SkillWatcherComponentSystem
    {
        [ObjectSystem]
        public class SkillWatcherComponentAwakeSystem : AwakeSystem<SkillWatcherComponent>
        {
            protected override void Awake(SkillWatcherComponent self)
            {
                SkillWatcherComponent.Instance = self;
                self.Init();
            }
        }

	
        public class SkillWatcherComponentLoadSystem : LoadSystem<SkillWatcherComponent>
        {
            protected override void Load(SkillWatcherComponent self)
            {
                self.Init();
            }
        }

        private static void Init(this SkillWatcherComponent self)
        {
            self.allWatchers = new Dictionary<ESkillEventType, List<ISkillWatcher>>();

            HashSet<Type> types = EventSystem.Instance.GetTypes(typeof(SkillWatcherAttribute));
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