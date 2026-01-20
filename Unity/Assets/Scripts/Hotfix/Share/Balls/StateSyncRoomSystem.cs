using System;
using System.Collections.Generic;
using System.IO;

namespace ET
{
    [FriendOf(typeof(StateSyncRoom))]
    [EntitySystemOf(typeof(StateSyncRoom))]
    public static partial class StateSyncRoomSystem
    {
        public static StateSyncRoom StateSyncRoom(this Entity entity)
        {
            return entity.IScene as StateSyncRoom;
        }
        
        public static void Init(this StateSyncRoom self, List<UnitInfo> unitInfos, long startTime, int frame = -1)
        {
            self.StartTime = startTime;
            for (int i = 0; i < unitInfos.Count; ++i)
            {
                UnitInfo unitInfo = unitInfos[i];
                self.PlayerIds.Add(unitInfo.UnitId);
            }
        }

        [EntitySystem]
        public static void Awake(this StateSyncRoom self)
        {
            self.PlayerIds = new List<long>();
        }

        [EntitySystem]
        public static void FixedUpdate(this StateSyncRoom self)
        {
            
        }

        [EntitySystem]
        public static void Destroy(this StateSyncRoom self)
        {
            
        }
        

    }
}