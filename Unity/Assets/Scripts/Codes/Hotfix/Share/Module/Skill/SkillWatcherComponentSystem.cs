using System;
using System.Collections.Generic;

namespace ET
{
    [FriendOf(typeof(SkillWatcherComponent))]
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
            self.allWatchers = new Dictionary<ESkillType, List<ISkillWatcher>>();

            HashSet<Type> types = EventSystem.Instance.GetTypes(typeof(SkillWatcherAttribute));
            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(SkillWatcherAttribute), false);

                foreach (object attr in attrs)
                {
                    SkillWatcherAttribute SkillWatcherAttribute = (SkillWatcherAttribute)attr;
                    ISkillWatcher obj = (ISkillWatcher)Activator.CreateInstance(type);
                    // ISkillWatcher SkillWatcherInfo = new ISkillWatcher(SkillWatcherAttribute.SceneType, obj);
                    if (!self.allWatchers.ContainsKey(SkillWatcherAttribute.SkillType))
                    {
                        self.allWatchers.Add(SkillWatcherAttribute.SkillType, new List<ISkillWatcher>());
                    }
                    self.allWatchers[SkillWatcherAttribute.SkillType].Add(obj);
                }
            }
        }

        public static void Run(this SkillWatcherComponent self, Unit unit, EventType.SkillEvent args)
        {
            List<ISkillWatcher> list;
            if (!self.allWatchers.TryGetValue(args.skillType, out list))
            {
                return;
            }

            foreach (ISkillWatcher SkillWatcher in list)
            {
                SkillWatcher.Run(unit, args);
            }
        }
    }

   
}