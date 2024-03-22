using System;
using System.Collections.Generic;

namespace ET
{
    /// <summary>
    /// 技能事件组件,分发监听
    /// </summary>
    [ComponentOf(typeof(Scene))]
    public class ActionEventComponent : Entity, IAwake/*, ILoad*/
    {
        [StaticField]
        public static ActionEventComponent Instance { get; set; }
		
        public Dictionary<EActionEventType, List<IActionEvent>> allWatchers;
    }
}