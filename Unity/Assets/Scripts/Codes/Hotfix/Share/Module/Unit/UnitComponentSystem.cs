using System.Collections.Generic;
using System.Linq;

namespace ET
{
	[ObjectSystem]
	public class UnitComponentAwakeSystem : AwakeSystem<UnitComponent>
	{
		protected override void Awake(UnitComponent self)
		{
            self.Awake();
		}
	}
	[ObjectSystem]
	public class UnitComponentFixedUpdateSystem : FixedUpdateSystem<UnitComponent>
	{
		protected override void FixedUpdate(UnitComponent self)
		{
			self.FixedUpdate();
		}
	}
	[ObjectSystem]
	public class UnitComponentLatedUpdateSystem : LateUpdateSystem<UnitComponent>
	{
		protected override void LateUpdate(UnitComponent self)
		{
			self.LateUpdate();
		}
	}
	
	[ObjectSystem]
	public class UnitComponentDestroySystem : DestroySystem<UnitComponent>
	{
		protected override void Destroy(UnitComponent self)
		{
            self.NeedSyncUnits.Dispose();
		}
	}

	[FriendOf(typeof(UnitComponent))]
	public static class UnitComponentSystem
	{
		public static void Awake(this UnitComponent self)
		{
            self.NeedSyncUnits = ListComponent<Unit>.Create();
        }
		public static void Add(this UnitComponent self, Unit unit)
		{
		}

		public static Unit Get(this UnitComponent self, long id)
		{
			Unit unit = self.GetChild<Unit>(id);
			return unit;
		}

		public static void Remove(this UnitComponent self, long id)
		{
			Unit unit = self.GetChild<Unit>(id);
			unit?.Dispose();
		}


        public static void AddSyncUnit(this UnitComponent self, Unit unit)
        {
            self.NeedSyncUnits.Add(unit);
        }

        public static void FixedUpdate(this UnitComponent self)
		{
			
		}

		public static void LateUpdate(this UnitComponent self)
		{
            //每帧结尾同步一次需要同步的单位，随后清理
#if DOTNET
            self.SyncUnit();
#endif
		}
        
        
        
		public static void SyncUnit(this UnitComponent self)
		{
#if DOTNET
            if (self.NeedSyncUnits.Count == 0)
                return;
			//同步单位状态（位置、方向、）
			List<UnitInfo> infos = new List<UnitInfo>();
            foreach (Unit unit in self.NeedSyncUnits)
            {
                UnitInfo info = Server.UnitHelper.CreateUnitInfo(unit);
                if(unit.IsDisposed || unit.Type != UnitType.Bullet)
	                continue;
                infos.Add(info);
            }

            M2C_SyncUnits syncUnits = new M2C_SyncUnits() { Units = infos };
            foreach (Entity entity in self.Children.Values)
            {
                Unit unit = entity as Unit;
                if(unit.IsDisposed || unit.Type != UnitType.Bullet)
                    continue;
                Server.MessageHelper.Broadcast(unit, syncUnits);
            }
            self.NeedSyncUnits.Clear();
#endif
		}
	}
}