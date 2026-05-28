using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    // 把 LoopScrollPrefabSourceInstance（Loader 程序集）的取物委托接到 HotfixView 这边，
    // 通过 Scene 的 ResourcesLoaderComponent 加载 item 预制体并实例化。
    // HotfixView 程序集不允许声明非 Const 字段（ET0004），所以 Scene 上下文用闭包捕获，每次 Bind 重写委托。
    // 在 UIComponent.Awake 里调用一次 Bind(self.Root())，切场景重建 UIComponent 时会再 Bind。
    public static class LoopScrollPoolBridge
    {
        public static void Bind(Scene scene)
        {
            EntityRef<Scene> sceneRef = scene;

            // LoopScrollRect 自身已通过 deletedItemType 池回收可见项，GetObject 只在可见窗口增长时调用，
            // 这里直接 Load+Instantiate，ReturnObject 时 Destroy，不再依赖场景里的 PoolRoot 节点。
            LoopScrollPrefabSourceInstance.OnInitPool = null;

            LoopScrollPrefabSourceInstance.OnGetFromPool = prefabName =>
            {
                Scene s = sceneRef;
                ResourcesLoaderComponent loader = s?.GetComponent<ResourcesLoaderComponent>();
                if (loader == null)
                {
                    Log.Error($"LoopScrollPoolBridge: scene 或 ResourcesLoaderComponent 为空，无法加载 {prefabName}");
                    return null;
                }
                GameObject prefab = loader.LoadAssetSync<GameObject>(prefabName);
                if (prefab == null)
                {
                    Log.Error($"LoopScrollPoolBridge: 加载 item 预制体失败 {prefabName}");
                    return null;
                }
                return UnityEngine.Object.Instantiate(prefab);
            };

            LoopScrollPrefabSourceInstance.OnReturnToPool = go =>
            {
                if (go != null)
                {
                    UnityEngine.Object.Destroy(go);
                }
            };
        }
    }
}
