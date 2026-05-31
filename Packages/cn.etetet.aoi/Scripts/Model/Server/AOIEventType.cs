namespace ET.Server
{
	public struct UnitEnterSightRange
	{
		public EntityRef<Unit> A;
		public EntityRef<Unit> B;
	}

	public struct UnitLeaveSightRange
	{
		public EntityRef<Unit> A;   // 观察者(始终有效)
		// 离开视野/被移除的单位 id —— 用 id 而非 EntityRef<Unit>:单位被销毁(UnitComponent.Remove)时,
		// AOIEntity.Destroy→LeaveSight 触发,此刻该单位 InstanceId 已被 Entity.Dispose 开头置 0,
		// 用 EntityRef<Unit> 隐式转换会抛 "entity is disposed"。处理器只需 id(发 M2C_RemoveUnits)。
		public long BId;
	}
}