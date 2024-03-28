using System;
using System.Collections.Generic;
using ET.EventType;

namespace ET
{
    [EntitySystemOf(typeof(ActionEventComponent))]
    [FriendOf(typeof(ActionEventComponent))]
    [FriendOf(typeof(ActionEvent))]
    public static partial class ActionEventComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ActionEventComponent self)
        {
            ActionEventComponent.Instance = self;
            self.allWatchers = new Dictionary<EActionEventType, List<IActionEvent>>();

            HashSet<Type> types = CodeTypes.Instance.GetTypes(typeof(ActionEventAttribute));
            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(ActionEventAttribute), false);

                foreach (object attr in attrs)
                {
                    ActionEventAttribute actionEventAttribute = (ActionEventAttribute)attr;
                    IActionEvent obj = (IActionEvent)Activator.CreateInstance(type);
                    if (!self.allWatchers.ContainsKey(actionEventAttribute.ActionEventType))
                    {
                        self.allWatchers.Add(actionEventAttribute.ActionEventType, new List<IActionEvent>());
                    }
                    self.allWatchers[actionEventAttribute.ActionEventType].Add(obj);
                }
            }
        }

        /// <summary>
        /// 技能时间轴执行到对应技能事件调用此分发，传入参数，去执行相对应事件逻辑
        /// </summary>
        /// <param name="self"></param>
        /// <param name="entity"></param>
        /// <param name="args"></param>
        public static void Run(this ActionEventComponent self, ActionEvent actionEvent, ActionEventData args)
        {
            List<IActionEvent> list;
            if (!self.allWatchers.TryGetValue(actionEvent.ActionEventType, out list))
            {
                return;
            }

            foreach (IActionEvent iActionEvent in list)
            {
                iActionEvent.Run(actionEvent, args);
            }
        }
    }

   
}