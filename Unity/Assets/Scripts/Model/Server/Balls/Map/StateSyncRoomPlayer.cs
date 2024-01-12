namespace ET.Server
{

    [ChildOf(typeof (RoomServerComponent))]
    public class StateSyncRoomPlayer : Entity, IAwake
    {
        public int Progress { get; set; }

        public bool IsOnline { get; set; } = true;
    }
}