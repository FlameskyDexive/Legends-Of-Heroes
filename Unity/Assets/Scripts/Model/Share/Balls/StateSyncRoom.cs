using System.Collections.Generic;

namespace ET
{
    [ComponentOf]
    public class StateSyncRoom : Entity, IScene, IAwake, IFixedUpdate, IDestroy
    {
        public Fiber Fiber { get; set; }
        public SceneType SceneType { get; set; } = SceneType.Room;
        public string Name { get; set; }

        public long StartTime { get; set; }

        public long RoomId { get; set; }

        public string RoomName { get; set; }

        public RoomMode Mode { get; set; }

        public int MaxPlayers { get; set; }

        public long CreatorId { get; set; }

        public string Password { get; set; }

        public RoomStatus Status { get; set; }

        public bool IsReady { get; set; }
        public List<long> PlayerIds { get; set; }
    }
}