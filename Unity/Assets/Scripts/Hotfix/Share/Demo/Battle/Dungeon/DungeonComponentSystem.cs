using System.Linq;

namespace ET
{
	[EntitySystemOf(typeof(DungeonComponent))]
	public static partial class DungeonComponentSystem
	{
		[EntitySystem]
		public static void Awake(this DungeonComponent self)
		{
		}
		

		[EntitySystem]
		public static void FixedUpdate(this DungeonComponent self)
		{
			self.SyncUnit();
		}
        
		public static void SyncUnit(this DungeonComponent self)
		{
			
		}
		[EntitySystem]
		public static void Destroy(this DungeonComponent self)
		{
			
		}
	}
}