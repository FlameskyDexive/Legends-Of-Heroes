namespace ET.Server
{
    [ChildOf(typeof(PlayerComponent))]
    public sealed class Player : Entity, IAwake<string>
    {
        public string Account { get; set; }
		
        public long UnitId { get; set; }
        
        public long RoomId { get; set; }
        public int CampId { get; set; }
        
        public string PlayerName { get; set; }
        public int AvatarIndex { get; set; }
    }
}