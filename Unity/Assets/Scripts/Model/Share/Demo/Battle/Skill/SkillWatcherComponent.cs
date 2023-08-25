using System;
using System.Collections.Generic;

namespace ET
{
    /// <summary>
    /// 技能事件组件,分发监听
    /// </summary>
    [Code]
    [FriendOf(typeof(SkillEvent))]
    public class SkillWatcherComponent : Singleton<SkillWatcherComponent>, ISingletonAwake
    {
		
        public Dictionary<ESkillEventType, List<ISkillWatcher>> allWatchers;

        public void Awake()
        {
            this.allWatchers = new Dictionary<ESkillEventType, List<ISkillWatcher>>();

            HashSet<Type> types = CodeTypes.Instance.GetTypes(typeof(SkillWatcherAttribute));
            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(SkillWatcherAttribute), false);

                foreach (object attr in attrs)
                {
                    SkillWatcherAttribute SkillWatcherAttribute = (SkillWatcherAttribute)attr;
                    ISkillWatcher obj = (ISkillWatcher)Activator.CreateInstance(type);
                    if (!this.allWatchers.ContainsKey(SkillWatcherAttribute.SkillEventType))
                    {
                        this.allWatchers.Add(SkillWatcherAttribute.SkillEventType, new List<ISkillWatcher>());
                    }
                    this.allWatchers[SkillWatcherAttribute.SkillEventType].Add(obj);
                }
            }
        }

        /// <summary>
        /// 技能时间轴执行到对应技能事件调用此分发，传入参数，去执行相对应事件逻辑
        /// </summary>
        /// <param name="self"></param>
        /// <param name="entity"></param>
        /// <param name="args"></param>
        public void Run(SkillEvent skillEvent, SkillEventType args)
        {
            List<ISkillWatcher> list;
            if (!this.allWatchers.TryGetValue(skillEvent.SkillEventType, out list))
            {
                return;
            }

            foreach (ISkillWatcher skillWatcher in list)
            {
                skillWatcher.Run(skillEvent, args);
            }
        }
    }
}