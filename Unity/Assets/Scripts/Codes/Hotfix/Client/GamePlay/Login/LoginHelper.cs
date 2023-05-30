using System;
using System.Net;
using System.Net.Sockets;

namespace ET.Client
{
    public static class LoginHelper
    {
        public static async ETTask<int> Login(Scene clientScene, string account, string password, bool isReconnect = false)
        {
            try
            {
                // 创建一个ETModel层的Session
                clientScene.RemoveComponent<RouterAddressComponent>();
                // 获取路由跟realmDispatcher地址
                RouterAddressComponent routerAddressComponent = clientScene.GetComponent<RouterAddressComponent>();
                if (routerAddressComponent == null)
                {
                    clientScene.RemoveComponent<NetClientComponent>();
                    routerAddressComponent = clientScene.AddComponent<RouterAddressComponent, string, int>(ConstValue.RouterHttpHost, ConstValue.RouterHttpPort);
                    await routerAddressComponent.Init();
                    
                    clientScene.AddComponent<NetClientComponent, AddressFamily>(routerAddressComponent.RouterManagerIPAddress.AddressFamily);
                }
                IPEndPoint realmAddress = routerAddressComponent.GetRealmAddress(account);
                
                R2C_Login r2CLogin;
                using (Session session = await RouterHelper.CreateRouterSession(clientScene, realmAddress))
                {
                    r2CLogin = (R2C_Login) await session.Call(new C2R_Login() { Account = account, Password = password });
                }

                // 创建一个gate Session,并且保存到SessionComponent中
                Session gateSession = await RouterHelper.CreateRouterSession(clientScene, NetworkHelper.ToIPEndPoint(r2CLogin.Address));
                if (isReconnect)
                {
                    clientScene.GetComponent<SessionComponent>().Session?.Dispose();
                    clientScene.GetComponent<SessionComponent>().Session = gateSession;
                }
                else
                {
                    clientScene.AddComponent<SessionComponent>().Session = gateSession;
                }
				
                G2C_LoginGate g2CLoginGate = (G2C_LoginGate)await gateSession.Call(
                    new C2G_LoginGate() { Key = r2CLogin.Key, GateId = r2CLogin.GateId});

                //此时缓存客户端全局的Player信息
                Log.Debug("登陆gate成功!");
                PlayerComponent playerComponent = clientScene.GetComponent<PlayerComponent>();

                playerComponent.MyId = g2CLoginGate.PlayerId;

                playerComponent.MyPlayer = new Player();
                playerComponent.MyPlayer.GateSession = gateSession;
                playerComponent.MyPlayer.PlayerId = playerComponent.MyId;
                playerComponent.MyPlayer.PlayerName = g2CLoginGate.PlayerName;
                playerComponent.MyPlayer.AvatarIndex = g2CLoginGate.AvatarIndex;
                playerComponent.MyPlayer.LobbyActorId = g2CLoginGate.LobbyActorId;

                PlayerComponent player = clientScene.GetComponent<PlayerComponent>();
                player.Account = account;
                player.Password = password;
                if (!isReconnect)
                {
                    // clientScene.GetComponent<ReconnectComponent>().Start
                    await EventSystem.Instance.PublishAsync(clientScene, new EventType.LoginFinish());
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                return ErrorCode.ERR_LoginError;
            }

            return ErrorCode.ERR_Success;
        } 
    }
}