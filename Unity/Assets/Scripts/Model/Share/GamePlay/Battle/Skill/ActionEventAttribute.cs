namespace ET
{
	public class ActionEventAttribute : BaseAttribute
    {
        public SceneType SceneType { get; }

        public EActionEventType ActionEventType { get; }

		public ActionEventAttribute(SceneType sceneType, EActionEventType eventType)
		{
            this.SceneType = sceneType;
			this.ActionEventType = eventType;
		}
	}
}