using UnityEngine.UI;

namespace ET.Client
{
    // 把 GameObjectPoolHelper（HotfixView 程序集）接到 LoopScrollPrefabSourceInstance（Loader 程序集），
    // 避免 Loader 反向依赖 HotfixView。
    // HotfixView 程序集不允许声明非 Const 字段（ET0004），因此 Scene 上下文通过闭包捕获，每次 Bind 重写委托即可。
    // 业务方在客户端 Scene 启动后调用一次 Bind(scene)；切场景再 Bind 一次。
    public static class LoopScrollPoolBridge
    {
        public static void Bind(Scene scene)
        {
            EntityRef<Scene> sceneRef = scene;

            LoopScrollPrefabSourceInstance.OnInitPool = (poolName, size) =>
            {
                Scene s = sceneRef;
                if (s == null) { return; }
                GameObjectPoolHelper.InitPool(s, poolName, size);
            };

            LoopScrollPrefabSourceInstance.OnGetFromPool = poolName =>
            {
                Scene s = sceneRef;
                if (s == null) { return null; }
                return GameObjectPoolHelper.GetObjectFromPool(s, poolName, true, 1);
            };

            LoopScrollPrefabSourceInstance.OnReturnToPool = GameObjectPoolHelper.ReturnObjectToPool;
        }
    }
}
