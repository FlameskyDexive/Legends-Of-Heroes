namespace ET.Server
{

    [ComponentOf(typeof (Player))]
    public class StateSyncPlayerRoomComponent : Entity, IAwake
    {
        public ActorId RoomActorId { get; set; }
    }
}