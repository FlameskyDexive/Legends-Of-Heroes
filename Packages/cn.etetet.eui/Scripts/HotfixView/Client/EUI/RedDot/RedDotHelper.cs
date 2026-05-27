using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof(RedDotComponent))]
    public static class RedDotHelper
    {
        public static void AddRedDotNode(Scene scene, string parent, string target, bool isNeedShowNum)
        {
            var comp = scene.GetComponent<RedDotComponent>();
            if (comp == null)
            {
                return;
            }
            if (!string.IsNullOrEmpty(parent) && !comp.RedDotNodeParentsDict.ContainsKey(parent))
            {
                Log.Warning("Runtime 动态添加的红点，其父节点是新节点: " + parent);
            }
            comp.AddRedDotNode(parent, target, isNeedShowNum);
        }

        public static void RemoveRedDotNode(Scene scene, string target, bool isRemoveView = true)
        {
            var comp = scene.GetComponent<RedDotComponent>();
            if (comp == null)
            {
                return;
            }
            comp.RemoveRedDotNode(target);
            if (isRemoveView)
            {
                comp.RemoveRedDotView(target, out _);
            }
        }

        public static void AddRedDotNodeView(Scene scene, string target, GameObject gameObject, Vector3 redDotScale, Vector2 positionOffset)
        {
            var comp = scene.GetComponent<RedDotComponent>();
            if (comp == null)
            {
                return;
            }
            var monoView = gameObject.GetComponent<RedDotMonoView>() ?? gameObject.AddComponent<RedDotMonoView>();
            monoView.RedDotScale = redDotScale;
            monoView.PositionOffset = positionOffset;
            comp.AddRedDotView(target, monoView);
        }

        public static void AddRedDotNodeView(Scene scene, string target, RedDotMonoView monoView)
        {
            var comp = scene.GetComponent<RedDotComponent>();
            if (comp == null)
            {
                return;
            }
            comp.AddRedDotView(target, monoView);
        }

        public static void RemoveRedDotView(Scene scene, string target, out RedDotMonoView monoView)
        {
            monoView = null;
            var comp = scene?.GetComponent<RedDotComponent>();
            if (comp == null)
            {
                return;
            }
            comp.RemoveRedDotView(target, out monoView);
        }

        public static bool HideRedDotNode(Scene scene, string target)
        {
            var comp = scene.GetComponent<RedDotComponent>();
            return comp != null && comp.HideRedDotNode(target);
        }

        public static bool ShowRedDotNode(Scene scene, string target)
        {
            if (IsLogicAlreadyShow(scene, target))
            {
                return false;
            }
            var comp = scene.GetComponent<RedDotComponent>();
            return comp != null && comp.ShowRedDotNode(target);
        }

        public static bool IsLogicAlreadyShow(Scene scene, string target)
        {
            var comp = scene.GetComponent<RedDotComponent>();
            if (comp == null)
            {
                Log.Error("RedDotComponent is not exist!");
                return false;
            }
            return comp.RedDotNodeRetainCount.TryGetValue(target, out int count) && count >= 1;
        }

        public static void RefreshRedDotViewCount(Scene scene, string target, int count)
        {
            var comp = scene.GetComponent<RedDotComponent>();
            if (comp == null)
            {
                return;
            }
            comp.RefreshRedDotViewCount(target, count);
        }
    }
}
