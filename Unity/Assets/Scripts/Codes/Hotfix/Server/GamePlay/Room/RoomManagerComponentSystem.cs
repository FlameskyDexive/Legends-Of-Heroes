using System.Collections;
using System.Collections.Generic;

namespace ET.Server
{

    [FriendOf(typeof(RoomManagerComponent))]
    public static class RoomManagerComponentSystem 
    {
        [ObjectSystem]
        public class RoomManagerComponentAwakeSystem : AwakeSystem<RoomManagerComponent>
        {
            protected override void Awake(RoomManagerComponent self)
            {

            }
        }
        
        [ObjectSystem]
        public class RoomManagerComponentDestroySystem : DestroySystem<RoomManagerComponent>
        {
            protected override void Destroy(RoomManagerComponent self)
            {
                self.Rooms.Clear();
            }
        }

        public static Room GetRoom(this RoomManagerComponent self, long id)
        {
            if (self.Rooms.TryGetValue(id, out Room room))
            {
                return room;
            }
            else
            {
                return null;
            }
        }

        public static void RemoveRoom(this RoomManagerComponent self, long id)
        {
            if (self.Rooms.TryGetValue(id, out Room room))
            {
                room.Dispose();
                self.Rooms.Remove(id);
            }
        }


    }
}
