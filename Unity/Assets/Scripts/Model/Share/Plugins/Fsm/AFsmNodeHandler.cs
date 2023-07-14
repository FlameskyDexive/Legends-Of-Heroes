namespace ET
{
    public class FsmNodeAttribute: BaseAttribute
    {
    }
    
    public abstract class AFsmNodeHandler
    {
        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name { get; set; }

        public virtual async ETTask OnEnter(FsmComponent fsmComponent)
        {
            await ETTask.CompletedTask;
        }

        public virtual void OnUpdate(FsmComponent fsmComponent)
        {
        }

        public virtual void OnExit(FsmComponent fsmComponent)
        {
            
        }
    }
}