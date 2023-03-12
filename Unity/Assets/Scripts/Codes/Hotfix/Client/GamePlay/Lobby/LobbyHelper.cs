using System;
using System.Collections;
using System.Collections.Generic;

namespace ET.Client
{
    public class LobbyHelper
    {
        public static async ETTask JoinOrCreateRoom(Scene clientScene)
        {
            try
            {
                PlayerComponent playerComponent = clientScene.GetComponent<PlayerComponent>();
                C2L_JoinOrCreateRoomLobby joinData = new C2L_JoinOrCreateRoomLobby() { PlayerId = playerComponent.MyId };
                // L2C_JoinOrCreateRoomLobby l2CJoinOrCreateRoomLobby = await clientScene.GetComponent<SessionComponent>().Session.Call(joinData) as L2C_JoinOrCreateRoomLobby;
                L2C_JoinOrCreateRoomLobby l2CJoinOrCreateRoomLobby = await playerComponent.LobbySession.Call(joinData) as L2C_JoinOrCreateRoomLobby;

                if (l2CJoinOrCreateRoomLobby.playerInfoRoom?.Count > 0)
                {
                    int curRoomPlayerCount = l2CJoinOrCreateRoomLobby.playerInfoRoom.Count;
                    
                    Log.Info($"room players:{l2CJoinOrCreateRoomLobby.playerInfoRoom.Count}");
                }
                
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

    }
}
