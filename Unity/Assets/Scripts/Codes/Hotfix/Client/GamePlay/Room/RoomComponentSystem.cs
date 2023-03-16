using System;

namespace ET.Client
{
    [FriendOf(typeof(RoomComponent))]
    public static class RoomComponentSystem
    {
        [ObjectSystem]
        public class RoomComponentAwakeSystem : AwakeSystem<RoomComponent>
        {
            protected override void Awake(RoomComponent self)
            {

            }

            private static void RoomAsync(RoomComponent self)
            {

            }
        }

        [ObjectSystem]
        public class RoomComponentDestroySystem : DestroySystem<RoomComponent>
        {
            protected override void Destroy(RoomComponent self)
            {
                self.PlayerInfos.Clear();
            }
        }

        public static void UpdateRoomPlayersInfo(this RoomComponent self, G2C_UpdateRoomPlayers roomPlayers)
        {
            self.PlayerInfos = roomPlayers.roomInfo.playerInfoRoom;
            Log.Info($"refresh room info, {self.PlayerInfos.Count}");
        }
        
    }
    
    
}