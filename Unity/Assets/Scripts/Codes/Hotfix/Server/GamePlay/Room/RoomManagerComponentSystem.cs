using System.Collections;
using System.Collections.Generic;

namespace ET.Server
{

    [FriendOf(typeof(RoomManagerComponent))]
    [FriendOf(typeof(Room))]
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


        public static Room CreateLobbyRoom(this RoomManagerComponent self, long playerId, long roomId, int maxPlayerNum)
        {
            Room room = self.AddChildWithId<Room>(roomId);
            room.RoomOwnerPlayerId = playerId;
            room.MaxPlayerNum = maxPlayerNum;
            room.CurPlayerNum = 0;
            room.AllPlayers.Clear();
            room.CampPlayers.Clear();

            self.Rooms.Add(room.Id, room);
            return room;
        }

        /// <summary>
        /// 获取一个可用的房间
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Room GetAvalibleRoom(this RoomManagerComponent self, long playerId, int maxPlayerNum)
        {
            if(self.Rooms.Count > 0)
            {
                foreach (Room room in self.Rooms.Values)
                {
                    if (room.MaxPlayerNum == maxPlayerNum && room.AllPlayers.Count < maxPlayerNum)
                        return room;
                }
            }

            return self.CreateLobbyRoom(playerId, self.RoomIdNum + 1, maxPlayerNum);
        }

    }
}
