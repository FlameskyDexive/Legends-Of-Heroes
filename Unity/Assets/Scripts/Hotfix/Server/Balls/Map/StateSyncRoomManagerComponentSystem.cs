using System;
using System.Collections.Generic;
using System.Linq;

namespace ET.Server
{
    [EntitySystemOf(typeof(StateSyncRoomManagerComponent))]
    [FriendOf(typeof(StateSyncRoomManagerComponent))]
    public static partial class StateSyncRoomManagerComponentSystem
    {
        [EntitySystem]
        private static void Awake(this StateSyncRoomManagerComponent self)
        {
            self.Rooms = new Dictionary<long, EntityRef<StateSyncRoom>>();
            self.RoomIdGenerator = 0;
            self.PlayerToRoom = new Dictionary<long, long>();
        }

        [EntitySystem]
        private static void Destroy(this StateSyncRoomManagerComponent self)
        {
            foreach (EntityRef<StateSyncRoom> roomRef in self.Rooms.Values)
            {
                StateSyncRoom room = roomRef;
                if (room != null && !room.IsDisposed)
                {
                    room.Dispose();
                }
            }
            self.Rooms.Clear();
            self.PlayerToRoom.Clear();
        }

        public static long GenerateRoomId(this StateSyncRoomManagerComponent self)
        {
            self.RoomIdGenerator++;
            return TimeInfo.Instance.ServerFrame() * 10000 + self.RoomIdGenerator;
        }

        public static StateSyncRoom GetRoom(this StateSyncRoomManagerComponent self, long roomId)
        {
            if (!self.Rooms.TryGetValue(roomId, out EntityRef<StateSyncRoom> roomRef))
            {
                return null;
            }
            return roomRef;
        }

        public static StateSyncRoom CreateRoom(this StateSyncRoomManagerComponent self, string roomName, RoomMode mode, int maxPlayers, string password, long creatorId)
        {
            long roomId = self.GenerateRoomId();

            StateSyncRoom room = self.AddChildWithId<StateSyncRoom>(roomId);
            room.Name = "Server";
            room.RoomId = roomId;
            room.RoomName = roomName;
            room.Mode = mode;
            room.MaxPlayers = maxPlayers;
            room.Password = password;
            room.CreatorId = creatorId;
            room.Status = RoomStatus.Waiting;
            room.IsReady = false;
            room.StartTime = TimeInfo.Instance.ServerNow();

            StateSyncRoomServerComponent serverComponent = room.AddComponent<StateSyncRoomServerComponent>();
            StateSyncRoomPlayer creatorPlayer = serverComponent.AddChildWithId<StateSyncRoomPlayer>(creatorId);
            creatorPlayer.IsCreator = true;
            creatorPlayer.IsOnline = true;
            creatorPlayer.IsReady = false;

            self.Rooms[roomId] = room;
            self.PlayerToRoom[creatorId] = roomId;

            return room;
        }

        public static bool JoinRoom(this StateSyncRoomManagerComponent self, long roomId, long playerId)
        {
            StateSyncRoom room = self.GetRoom(roomId);
            if (room == null || room.IsDisposed)
            {
                return false;
            }

            if (room.Status != RoomStatus.Waiting)
            {
                return false;
            }

            StateSyncRoomServerComponent serverComponent = room.GetComponent<StateSyncRoomServerComponent>();
            if (serverComponent == null || serverComponent.IsDisposed)
            {
                return false;
            }

            if (serverComponent.Children.Count >= room.MaxPlayers)
            {
                return false;
            }

            if (serverComponent.Children.ContainsKey(playerId))
            {
                return false;
            }

            StateSyncRoomPlayer roomPlayer = serverComponent.AddChildWithId<StateSyncRoomPlayer>(playerId);
            roomPlayer.IsCreator = false;
            roomPlayer.IsOnline = true;
            roomPlayer.IsReady = false;

            self.PlayerToRoom[playerId] = roomId;

            return true;
        }

        public static bool LeaveRoom(this StateSyncRoomManagerComponent self, long playerId)
        {
            if (!self.PlayerToRoom.TryGetValue(playerId, out long roomId))
            {
                return false;
            }

            StateSyncRoom room = self.GetRoom(roomId);
            if (room == null || room.IsDisposed)
            {
                self.PlayerToRoom.Remove(playerId);
                return false;
            }

            StateSyncRoomServerComponent serverComponent = room.GetComponent<StateSyncRoomServerComponent>();
            if (serverComponent == null || serverComponent.IsDisposed)
            {
                self.PlayerToRoom.Remove(playerId);
                return false;
            }

            StateSyncRoomPlayer roomPlayer = serverComponent.GetChild<StateSyncRoomPlayer>(playerId);
            if (roomPlayer != null && !roomPlayer.IsDisposed)
            {
                roomPlayer.Dispose();
            }

            self.PlayerToRoom.Remove(playerId);

            if (playerId == room.CreatorId || serverComponent.Children.Count == 0)
            {
                self.DestroyRoom(roomId);
            }

            return true;
        }

        public static void DestroyRoom(this StateSyncRoomManagerComponent self, long roomId)
        {
            StateSyncRoom room = self.GetRoom(roomId);
            if (room == null || room.IsDisposed)
            {
                return;
            }

            StateSyncRoomServerComponent serverComponent = room.GetComponent<StateSyncRoomServerComponent>();
            if (serverComponent != null && !serverComponent.IsDisposed)
            {
                foreach (StateSyncRoomPlayer roomPlayer in serverComponent.Children.Values.ToList())
                {
                    if (roomPlayer != null && !roomPlayer.IsDisposed)
                    {
                        self.PlayerToRoom.Remove(roomPlayer.Id);
                    }
                }
            }

            room.Dispose();
            self.Rooms.Remove(roomId);
        }

        public static RoomInfo GetRoomInfo(this StateSyncRoomManagerComponent self, long roomId)
        {
            StateSyncRoom room = self.GetRoom(roomId);
            if (room == null || room.IsDisposed)
            {
                return null;
            }

            RoomInfo roomInfo = RoomInfo.Create();
            roomInfo.RoomId = room.RoomId;
            roomInfo.RoomName = room.RoomName;
            roomInfo.Mode = room.Mode;
            roomInfo.MaxPlayers = room.MaxPlayers;
            roomInfo.CreatorId = room.CreatorId;
            roomInfo.Status = room.Status;
            roomInfo.IsReady = room.IsReady;
            roomInfo.Password = room.Password;

            StateSyncRoomServerComponent serverComponent = room.GetComponent<StateSyncRoomServerComponent>();
            if (serverComponent != null && !serverComponent.IsDisposed)
            {
                foreach (StateSyncRoomPlayer roomPlayer in serverComponent.Children.Values)
                {
                    if (roomPlayer == null || roomPlayer.IsDisposed)
                    {
                        continue;
                    }

                    PlayerInfo playerInfo = PlayerInfo.Create();
                    playerInfo.PlayerId = roomPlayer.Id;
                    roomInfo.PlayerInfo.Add(playerInfo);
                }
            }

            return roomInfo;
        }

        public static List<RoomInfo> GetRoomList(this StateSyncRoomManagerComponent self, RoomMode? mode = null)
        {
            List<RoomInfo> roomList = new List<RoomInfo>();

            foreach (EntityRef<StateSyncRoom> roomRef in self.Rooms.Values)
            {
                StateSyncRoom room = roomRef;
                if (room == null || room.IsDisposed)
                {
                    continue;
                }

                if (mode.HasValue && room.Mode != mode.Value)
                {
                    continue;
                }

                RoomInfo roomInfo = self.GetRoomInfo(room.RoomId);
                if (roomInfo != null)
                {
                    roomList.Add(roomInfo);
                }
            }

            return roomList;
        }

        public static long GetPlayerRoomId(this StateSyncRoomManagerComponent self, long playerId)
        {
            if (self.PlayerToRoom.TryGetValue(playerId, out long roomId))
            {
                return roomId;
            }
            return 0;
        }

        public static bool UpdateRoomReady(this StateSyncRoomManagerComponent self, long roomId, bool isReady)
        {
            StateSyncRoom room = self.GetRoom(roomId);
            if (room == null || room.IsDisposed)
            {
                return false;
            }

            room.IsReady = isReady;
            if (isReady)
            {
                room.Status = RoomStatus.Ready;
            }
            else
            {
                room.Status = RoomStatus.Waiting;
            }

            return true;
        }

        public static bool UpdateRoomStatus(this StateSyncRoomManagerComponent self, long roomId, RoomStatus status)
        {
            StateSyncRoom room = self.GetRoom(roomId);
            if (room == null || room.IsDisposed)
            {
                return false;
            }

            room.Status = status;
            return true;
        }
    }
}