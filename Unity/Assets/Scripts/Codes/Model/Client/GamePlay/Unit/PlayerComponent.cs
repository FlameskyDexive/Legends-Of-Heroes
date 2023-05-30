namespace ET.Client
{
    [ComponentOf(typeof(Scene))]
    public class PlayerComponent: Entity, IAwake
    {
        public long MyId { get; set; }
        
        public Player MyPlayer { get; set; }
        
        public string Account { get; set; }
        public string Password { get; set; }
    }
}