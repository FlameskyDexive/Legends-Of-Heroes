using System;
using System.Collections;
using System.Collections.Generic;

namespace ET.Client
{
    public static class LobbyHelper
    {
        public static async ETTask JoinOrCreateRoom(Scene clientScene)
        {
            try
            {
                PlayerComponent playerComponent = clientScene.GetComponent<PlayerComponent>();
                C2G_JoinOrCreateRoom joinData = new C2G_JoinOrCreateRoom() { PlayerId = playerComponent.MyId };
                G2C_JoinOrCreateRoom l2CJoinOrCreateRoomLobby = await clientScene.GetComponent<SessionComponent>().Session.Call(joinData) as G2C_JoinOrCreateRoom;
                
                if (l2CJoinOrCreateRoomLobby.roomInfo?.playerInfoRoom?.Count > 0)
                {
                    int curRoomPlayerCount = l2CJoinOrCreateRoomLobby.roomInfo.playerInfoRoom.Count;
                    
                    Log.Info($"room players:{l2CJoinOrCreateRoomLobby.roomInfo.playerInfoRoom.Count}");
                }

                //测试actor消息，当前有点问题，通信异常，暂时用网关转发。
                C2L_JoinOrCreateRoom c2LJoinOrCreateRoom = new C2L_JoinOrCreateRoom() { PlayerId = playerComponent.MyId };
                clientScene.GetComponent<SessionComponent>().Session.Send(playerComponent.MyPlayer.LobbyActorId, c2LJoinOrCreateRoom);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            await ETTask.CompletedTask;
        }

    }
}
