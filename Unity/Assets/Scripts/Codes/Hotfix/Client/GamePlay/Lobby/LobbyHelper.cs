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
                C2G_JoinOrCreateRoom joinData = new C2G_JoinOrCreateRoom() { PlayerId = playerComponent.MyId };
                G2C_JoinOrCreateRoom l2CJoinOrCreateRoomLobby = await clientScene.GetComponent<SessionComponent>().Session.Call(joinData) as G2C_JoinOrCreateRoom;

                if (l2CJoinOrCreateRoomLobby.playerInfoRoom?.Count > 0)
                {
                    int curRoomPlayerCount = l2CJoinOrCreateRoomLobby.playerInfoRoom.Count;
                    
                    Log.Info($"room players:{l2CJoinOrCreateRoomLobby.playerInfoRoom.Count}");
                }
                
                //≤‚ ‘actorœ˚œ¢°£
                //clientScene.GetComponent<SessionComponent>().Session.Send(new C2L_JoinOrCreateRoom(){PlayerId = playerComponent.MyId});
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            await ETTask.CompletedTask;
        }

    }
}
