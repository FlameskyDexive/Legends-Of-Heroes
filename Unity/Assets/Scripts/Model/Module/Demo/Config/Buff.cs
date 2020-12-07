namespace ETModel
{
	[Config((int)(AppType.ClientH |  AppType.ClientM | AppType.Gate | AppType.Map))]
	public partial class BuffCategory : ACategory<Buff>
	{
	}

	public class Buff: IConfig
	{
		public long Id { get; set; }
		public int buffId;
		public string name;
		public string desc;
		public int buffType;
		public int skillLevel;
		public int opportunity;
		public int eventId;
	}
}
