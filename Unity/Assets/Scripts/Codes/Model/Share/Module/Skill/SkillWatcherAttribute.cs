namespace ET
{
	public class SkillWatcherAttribute : BaseAttribute
	{
		
		public ESkillType SkillType { get; }

		public SkillWatcherAttribute(ESkillType type)
		{
			this.SkillType = type;
		}
	}
}