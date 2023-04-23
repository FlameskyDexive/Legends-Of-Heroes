namespace ET
{
	
	[ComponentOf(typeof(Scene))]
    public class UnitComponent: Entity, IAwake, IFixedUpdate, ILateUpdate, IDestroy
    {
        public ListComponent<Unit> NeedSyncUnits;
    }
}