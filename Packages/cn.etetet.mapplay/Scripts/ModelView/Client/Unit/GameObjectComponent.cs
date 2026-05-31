using UnityEngine;

namespace ET.Client
{
    [ComponentOf(typeof(Unit))]
    public class GameObjectComponent: Entity, IAwake, IDestroy
    {
        private GameObject gameObject;

        public GameObject GameObject
        {
            get
            {
                return this.gameObject;
            }
            set
            {
                this.gameObject = value;
                this.Transform = value.transform;
            }
        }

        public Transform Transform { get; private set; }
    }

    // 客户端"贴地"射线(GameObjectPosHelper.OnTerrain)的全局开关。放在 ModelView(非 Hotfix 层):
    // Hotfix 程序集禁止声明非 const 字段(ET0004,热重载会丢静态状态),故静态可变开关必须放这里。
    // 平面玩法(球球大作战)进图时置 false(见 BallCameraComponentSystem):单位保持服务端 Y=0、且省掉每次移动的射线;离开还原 true。
    public static class GameObjectPosConfig
    {
        [StaticField]
        public static bool EnableTerrain = true;
    }
}