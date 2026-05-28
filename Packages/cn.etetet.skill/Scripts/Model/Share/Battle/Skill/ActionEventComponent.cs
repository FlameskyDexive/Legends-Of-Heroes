using System;
using System.Collections.Generic;

namespace ET
{
    public struct ActionEventInfo
    {
        // 本项目 SceneType 为静态类(int 常量),故用 int
        public int SceneType { get; }
        public IActionEvent IActionEvent { get; }

        public ActionEventInfo(int sceneType, IActionEvent actionEvent)
        {
            this.SceneType = sceneType;
            this.IActionEvent = actionEvent;
        }
    }

    /// <summary>
    /// 技能事件组件,分发监听
    /// </summary>
    [CodeProcess]
    [AllowInstance]
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

            int unitDomainSceneType = actionEvent.IScene.SceneType;
            foreach (ActionEventInfo actionEventInfo in list)
            {
                // SceneType==0 视为 All 通配;否则需与场景类型一致(本项目场景类型为唯一 int,非位标志)
                if (actionEventInfo.SceneType != 0 && actionEventInfo.SceneType != unitDomainSceneType)
                {
                    continue;
                }
                actionEventInfo.IActionEvent.Run(actionEvent, args);
            }
        }

    }
}