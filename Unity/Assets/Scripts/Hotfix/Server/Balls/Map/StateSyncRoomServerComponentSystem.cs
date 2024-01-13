using System;
using System.Collections.Generic;

namespace ET.Server
{
    [EntitySystemOf(typeof(StateSyncRoomServerComponent))]
    [FriendOf(typeof(StateSyncRoomServerComponent))]
    public static partial class StateSyncRoomServerComponentSystem
    {
        [EntitySystem]
        private static void Awake(this StateSyncRoomServerComponent self, List<long> playerIds)
        {
            foreach (long id in playerIds)
            {
                StateSyncRoomPlayer roomPlayer = self.AddChildWithId<StateSyncRoomPlayer>(id);
            }
        }

        public static bool IsAllPlayerProgress100(this StateSyncRoomServerComponent self)
        {
            foreach (StateSyncRoomPlayer roomPlayer in self.Children.Values)
            {
                if (roomPlayer.Progress != 100)
                {
                    return false;
                }
            }
            return true;
        }
    }
}