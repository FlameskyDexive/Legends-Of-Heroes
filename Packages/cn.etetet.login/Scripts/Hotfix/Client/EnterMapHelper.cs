using System;


namespace ET.Client
{
    public static partial class EnterMapHelper
    {
        public static async ETTask EnterMapAsync(Scene root)
        {
            try
            {
                EntityRef<Scene> rootRef = root;
                G2C_EnterMap g2CEnterMap = await root.GetComponent<ClientSenderComponent>().Call(C2G_EnterMap.Create()) as G2C_EnterMap;
                // 等待场景切换完成
                root = rootRef;
                await root.GetComponent<ObjectWait>().Wait<Wait_SceneChangeFinish>();
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
                await root.GetComponent<ClientSenderComponent>().Call(C2G_RequestMatchRobot.Create());
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}