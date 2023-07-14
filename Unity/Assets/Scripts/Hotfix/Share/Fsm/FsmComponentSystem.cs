using System;

namespace ET
{
    [ObjectSystem]
    public class FsmComponentAwakeSystem: AwakeSystem<FsmComponent, ETTask>
    {
        protected override void Awake(FsmComponent self, ETTask task)
        {
            self.Awake(task);
        }
    }
    
    [ObjectSystem]
    public class FsmComponentUpdateSystem: UpdateSystem<FsmComponent>
    {
        protected override void Update(FsmComponent self)
        {
            self.Update();
        }
    }
    
    [ObjectSystem]
    public class FsmComponentDestroySystem: DestroySystem<FsmComponent>
    {
        protected override void Destroy(FsmComponent self)
        {
            self.Destroy();
        }
    }

    [FriendOf(typeof(FsmComponent))]
    [FriendOf(typeof(FsmDispatcherComponent))]
    public static class FsmComponentSystem
    {
        /// <summary>
        /// 启动状态机
        /// </summary>
        /// <param name="self"></param>
        /// <param name="entryNode">入口节点</param>
        public static void Awake(this FsmComponent self, ETTask task)
        {
            self.DoneTask = task;
        }

        /// <summary>
        /// 更新状态机
        /// </summary>
        public static void Update(this FsmComponent self)
        {
            if (self.CurNodeHandler != null)
                self.CurNodeHandler.OnUpdate(self);
        }

        public static void Destroy(this FsmComponent self)
        {
            self.NodeHandlers.Clear();
            self.CurNodeHandler = null;
            self.PreNodeHandler = null;

            self.DoneTask.SetResult();
        }

        /// <summary>
        /// 加入一个节点
        /// </summary>
        public static void AddNodeHandler(this FsmComponent self, string nodeHandlerName)
        {
            FsmDispatcherComponent.Instance.FsmNodeHandlers.TryGetValue(nodeHandlerName, out AFsmNodeHandler handler);

            if (handler == null)
            {
                Log.Error("not found FsmNodeHandler: {0}".Fmt(nodeHandlerName));
                return;
            }

            if (self.NodeHandlers.ContainsKey(nodeHandlerName) == false)
            {
                self.NodeHandlers.Add(nodeHandlerName, handler);
            }
            else
            {
                Log.Warning("Node {0} already existed".Fmt(handler.Name));
            }
        }

        public static void Run(this FsmComponent self, string entryNodeHandlerName)
        {
            self.CurNodeHandler = self.GetNodeHandler(entryNodeHandlerName);
            self.PreNodeHandler = self.CurNodeHandler;

            if (self.CurNodeHandler != null)
                self.CurNodeHandler.OnEnter(self).Coroutine();
            else
                Log.Error("Not found entry node: {0}".Fmt(entryNodeHandlerName));
        }

        /// <summary>
        /// 转换节点
        /// </summary>
        public static void Transition(this FsmComponent self, string nodeHandlerName)
        {
            if (string.IsNullOrEmpty(nodeHandlerName))
                throw new ArgumentNullException();

            AFsmNodeHandler nodeHandler = self.GetNodeHandler(nodeHandlerName);
            if (nodeHandler == null)
            {
                Log.Error("not found FsmNode: {0}".Fmt(nodeHandlerName));
                return;
            }

            Log.Info("FSM change {0} to {1}".Fmt(self.CurNodeHandler.Name, nodeHandler.Name));
            self.PreNodeHandler = self.CurNodeHandler;
            self.CurNodeHandler.OnExit(self);
            self.CurNodeHandler = nodeHandler;
            self.CurNodeHandler.OnEnter(self).Coroutine();
        }

        /// <summary>
        /// 返回到之前的节点
        /// </summary>
        public static void RevertToPreviousNodeHandler(this FsmComponent self)
        {
            self.Transition(self.PreviousNodeHandlerName);
        }

        private static bool IsContains(this FsmComponent self, string nodeName)
        {
            return self.NodeHandlers.ContainsKey(nodeName);
        }

        private static AFsmNodeHandler GetNodeHandler(this FsmComponent self, string nodeName)
        {
            self.NodeHandlers.TryGetValue(nodeName, out AFsmNodeHandler ret);
            return ret;
        }
    }
}