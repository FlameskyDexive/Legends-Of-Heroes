namespace ET
{
	public class ActionEventAttribute : BaseAttribute
    {
        // 本项目 SceneType 为静态类(int 常量),故用 int 存场景类型（与 [Event(int)] 一致）
        public int SceneType { get; }

        public EActionEventType ActionEventType { get; }

		public ActionEventAttribute(int sceneType, EActionEventType eventType)
		{
            this.SceneType = sceneType;
			this.ActionEventType = eventType;
		}
	}
}