namespace ET
{
	public interface ISkillWatcher
	{
		void Run(SkillEvent skillEvent, EventType.SkillEventType args);
	}
}
