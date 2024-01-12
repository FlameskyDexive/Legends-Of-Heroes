using System.Collections.Generic;

namespace ET
{
    [ComponentOf]
    public class StateSyncRoom : Entity, IScene, IAwake, IUpdate
    {
        public Fiber Fiber { get; set; }
        public SceneType SceneType { get; set; } = SceneType.Room;
        public string Name { get; set; }
        
        public long StartTime { get; set; }

        // 玩家id列表
        public List<long> PlayerIds { get; } = new(ConstValue.StateSyncMatchCount);

        // 存档
        public Replay Replay { get; set; } = new();

        private EntityRef<StateSyncWorld> world;

        // World做成child，可以有多个World，比如守望先锋有两个
        public StateSyncWorld World
        {
            get
            {
                return this.world;
            }
            set
            {
                this.AddChild(value);
                this.world = value;
            }
        }

        public bool IsReplay { get; set; }
        
        public int SpeedMultiply { get; set; }
    }
}