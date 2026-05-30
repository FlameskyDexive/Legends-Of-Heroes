using System;


namespace ET.Client
{
    public static partial class EnterMapHelper
    {
        public static async ETTask EnterMapAsync(Scene root, string mapName = "Map2")
        {
            try
            {
                EntityRef<Scene> rootRef = root;
                C2G_EnterMap c2GEnterMap = C2G_EnterMap.Create();
                c2GEnterMap.MapName = mapName;
                Log.Info($"[进图诊断] 发 C2G_EnterMap MapName={mapName}, 等 G2C_EnterMap");
                G2C_EnterMap g2CEnterMap = await root.GetComponent<ClientSenderComponent>().Call(c2GEnterMap) as G2C_EnterMap;
                Log.Info($"[进图诊断] 收到 G2C_EnterMap(Error={g2CEnterMap?.Error}), 等场景切换 Wait_SceneChangeFinish");
                // 等待场景切换完成
                root = rootRef;
                await root.GetComponent<ObjectWait>().Wait<Wait_SceneChangeFinish>();
                Log.Info("[进图诊断] 场景切换完成 -> 发 EnterMapFinish");
                root = rootRef;
                EventSystem.Instance.Publish(root, new EnterMapFinish());
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        /// <summary>
        /// 匹配超时兜底：请求服务端创建一个机器人玩家当对手。
        /// 服务端 Gate 收到后用 RobotManager 创建机器人，机器人登录并进同一张共享地图。
        /// </summary>
        public static async ETTask RequestMatchRobotAsync(Scene root)
        {
            try
            {
                Log.Info("[机器人诊断] 发 C2G_RequestMatchRobot, 等响应");
                await root.GetComponent<ClientSenderComponent>().Call(C2G_RequestMatchRobot.Create());
                Log.Info("[机器人诊断] 收到 G2C_RequestMatchRobot 响应");
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}