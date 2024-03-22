namespace ET
{
	public class ActionEventAttribute : BaseAttribute
	{
		
		public EActionEventType ActionEventType { get; }

		public ActionEventAttribute(EActionEventType eventType)
		{
			this.ActionEventType = eventType;
		}
	}
}