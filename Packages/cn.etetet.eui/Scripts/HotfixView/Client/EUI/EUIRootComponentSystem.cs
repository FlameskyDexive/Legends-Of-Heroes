using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(EUIRootComponent))]
    [FriendOf(typeof(EUIRootComponent))]
    public static partial class EUIRootComponentSystem
    {
        // Init 场景已固化的 UIRoot 层级路径
        private const string PATH_UI_PARENT = "/Global/UI";
        private const string PATH_NORMAL_ROOT = "/Global/UI/NormalRoot";
        private const string PATH_FIXED_ROOT = "/Global/UI/FixedRoot";
        private const string PATH_POPUP_ROOT = "/Global/UI/PopUpRoot";
        private const string PATH_OTHER_ROOT = "/Global/UI/OtherRoot";

        [EntitySystem]
        private static void Awake(this EUIRootComponent self)
        {
            // 静态绑定：UIRoot 层级由 Init 场景固化（Global/UI/*）。
            // 这里只做一次性 Find，找不到时报错；不再动态创建 Canvas / DontDestroyOnLoad。
            self.Root = FindRoot(PATH_UI_PARENT);
            self.NormalRoot = FindRoot(PATH_NORMAL_ROOT);
            self.FixedRoot = FindRoot(PATH_FIXED_ROOT);
            self.PopUpRoot = FindRoot(PATH_POPUP_ROOT);
            self.OtherRoot = FindRoot(PATH_OTHER_ROOT);
        }

        [EntitySystem]
        private static void Destroy(this EUIRootComponent self)
        {
            // 静态绑定的场景节点不归 EUIRootComponent 负责销毁，置空引用即可。
            self.Root = null;
            self.NormalRoot = null;
            self.FixedRoot = null;
            self.PopUpRoot = null;
            self.OtherRoot = null;
        }

        private static Transform FindRoot(string path)
        {
            var go = GameObject.Find(path);
            if (go == null)
            {
                Log.Error($"[EUIRoot] 找不到场景节点: {path}，请确认 Init 场景已挂好 UIRoot 层级");
                return null;
            }
            return go.transform;
        }
    }
}
