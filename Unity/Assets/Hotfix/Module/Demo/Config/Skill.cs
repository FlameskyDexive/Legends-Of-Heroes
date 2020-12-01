using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.ClientH |  AppType.ClientM | AppType.Gate | AppType.Map))]
	public partial class SkillCategory : ACategory<Skill>
	{
	}

	public class Skill: IConfig
	{
		public long Id { get; set; }
		public int skillId;
		public string name;
		public string desc;
		public int skillType;
		public int skillLevel;
		public int opportunity;
		public int eventId;
	}
}
