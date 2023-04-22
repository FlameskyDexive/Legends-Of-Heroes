using System.Linq;

namespace ET
{
	[ObjectSystem]
	public class DungeonComponentAwakeSystem : AwakeSystem<DungeonComponent>
	{
		protected override void Awake(DungeonComponent self)
		{
		}
	}
	[ObjectSystem]
	public class DungeonComponentFixedUpdateSystem : FixedUpdateSystem<DungeonComponent>
	{
		protected override void FixedUpdate(DungeonComponent self)
		{
			self.FixedUpdate();
		}
	}
	
	[ObjectSystem]
	public class DungeonComponentDestroySystem : DestroySystem<DungeonComponent>
	{
		protected override void Destroy(DungeonComponent self)
		{
		}
	}
	
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
	}
}