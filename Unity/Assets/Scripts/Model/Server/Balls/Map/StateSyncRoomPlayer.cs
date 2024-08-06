namespace ET.Server
{

    [ChildOf(typeof (StateSyncRoomServerComponent))]
    public class StateSyncRoomPlayer : Entity, IAwake
    {
        public int Progress { get; set; }

        public bool IsOnline { get; set; } = true;
        
        public EntityRef<Unit> Unit { get; set; }
    }
}