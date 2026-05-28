using ET.EventType;
using System;
using System.Collections.Generic;

namespace ET
{
    public struct ActionEventInfo
    {
        public SceneType SceneType { get; }
        public IActionEvent IActionEvent { get; }

        public ActionEventInfo(SceneType sceneType, IActionEvent actionEvent)
        {
            this.SceneType = sceneType;
            this.IActionEvent = actionEvent;
        }
    }

    /// <summary>
    /// 技能事件组件,分发监听
    /// </summary>
    [Code]
    [FriendOf(typeof(ET.ActionEvent))]
    public class ActionEventComponent : Singleton<ActionEventComponent>, ISingletonAwake
    {

        private readonly Dictionary<EActionEventType, List<ActionEventInfo>> allWatchers = new();

        public void Awake()
        {
            HashSet<Type> types = CodeTypes.Instance.GetTypes(typeof(ActionEventAttribute));
            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(ActionEventAttribute), false);

                foreach (object attr in attrs)
                {
                    ActionEventAttribute actionEventAttribute = (ActionEventAttribute)attr;
                    IActionEvent obj = (IActionEvent)Activator.CreateInstance(type);
                    ActionEventInfo actionInfo = new ActionEventInfo(actionEventAttribute.SceneType, obj);
                    if (!this.allWatchers.ContainsKey(actionEventAttribute.ActionEventType))
                    {
                        this.allWatchers.Add(actionEventAttribute.ActionEventType, new List<ActionEventInfo>());
                    }
                    this.allWatchers[actionEventAttribute.ActionEventType].Add(actionInfo);
                }
            }
        }

        public void Run(ActionEvent actionEvent, ActionEventData args)
        {
            List<ActionEventInfo> list;
            if (!this.allWatchers.TryGetValue(actionEvent.ActionEventType, out list))
            {
                return;
            }

            SceneType unitDomainSceneType = actionEvent.IScene.SceneType;
            foreach (ActionEventInfo actionEventInfo in list)
            {
                if (!actionEventInfo.SceneType.HasSameFlag(unitDomainSceneType))
                {
                    continue;
                }
                actionEventInfo.IActionEvent.Run(actionEvent, args);
            }
        }

    }
}