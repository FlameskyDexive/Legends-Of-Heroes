namespace ET.Client
{
    // [ChildOf(typeof(PlayerComponent))]
    public sealed class Player /*: Entity, IAwake<long>*/
    {
        public long PlayerId { get; set; }
        public string Account { get; set; }
		
        
        public long RoomId { get; set; }
        public int CampId { get; set; }
        
        public string PlayerName { get; set; }
        public int AvatarIndex { get; set; }
        public long LobbyActorId { get; set; }
        public Session GateSession { get; set; }
    }
}