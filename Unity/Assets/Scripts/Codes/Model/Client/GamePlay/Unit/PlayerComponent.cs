namespace ET.Client
{
    [ComponentOf(typeof(Scene))]
    public class PlayerComponent: Entity, IAwake
    {
        public Session GateSession;
        public Session LobbySession;
        public long MyId { get; set; }
        
        public Player MyPlayer { get; set; }
    }
}