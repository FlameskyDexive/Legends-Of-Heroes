using System;
using System.Collections;
using System.Collections.Generic;

namespace ET
{
    /// <summary>
    /// 有限状态机
    /// </summary>
    [ComponentOf]
    public class FsmComponent: Entity, IAwake<ETTask>, IUpdate, IDestroy
    {
        public readonly Dictionary<string, AFsmNodeHandler> NodeHandlers = new Dictionary<string, AFsmNodeHandler>();
		
        public AFsmNodeHandler CurNodeHandler;
		
        public AFsmNodeHandler PreNodeHandler;

        public ETTask DoneTask;

        /// <summary>
        /// 当前运行的节点名称
        /// </summary>
        public string CurrentNodeHandlerName
        {
            get { return this.CurNodeHandler != null ? nameof(this.CurNodeHandler) : string.Empty; }
        }

        /// <summary>
        /// 之前运行的节点名称
        /// </summary>
        public string PreviousNodeHandlerName
        {
            get { return this.PreNodeHandler != null ? nameof(this.PreNodeHandler) : string.Empty; }
        }
    }
}