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

        // 玩家id列表
        public List<long> PlayerIds { get; } = new(ConstValue.StateSyncMatchCount);
    }
}