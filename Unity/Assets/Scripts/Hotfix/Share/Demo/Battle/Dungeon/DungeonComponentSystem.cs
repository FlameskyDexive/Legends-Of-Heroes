using System.Linq;

namespace ET
{

	public static class DungeonComponentSystem
	{
		public static void Awake(this DungeonComponent self)
		{
		}
		

		public static void FixedUpdate(this DungeonComponent self)
		{
			self.SyncUnit();
		}
        
		public static void SyncUnit(this DungeonComponent self)
		{
			
		}
		public static void Destroy(this DungeonComponent self)
		{
			
		}
	}
}