namespace ET
{
	public interface IActionEvent
	{
		void Run(ActionEvent actionEvent, EventType.ActionEventData args);
	}
}
