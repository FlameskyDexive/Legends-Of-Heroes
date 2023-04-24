using System.Collections;
using System.Collections.Generic;

namespace ET.Server
{

    [FriendOf(typeof(Room))]
    public static class RoomSystem
    {
        [ObjectSystem]
        public class RoomAwakeSystem : AwakeSystem<Room>
        {
            protected override void Awake(Room self)
            {
                
            }
        }
        [ObjectSystem]
        public class RoomDestroySystem : DestroySystem<Room>
        {
            protected override void Destroy(Room self)
            {
                self.AllPlayers.Clear();
                self.CampPlayers.Clear();
            }
        }
    }
}
