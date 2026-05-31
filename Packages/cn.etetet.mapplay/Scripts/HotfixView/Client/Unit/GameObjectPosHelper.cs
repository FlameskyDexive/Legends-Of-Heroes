using Unity.Mathematics;
using UnityEngine;

namespace ET.Client
{
    public static class GameObjectPosHelper
    {
        // 把朝向同步到名为 "RootDir" 的子节点(递归查找,RootDir 常嵌在 Root 等中间节点下);没有 RootDir 则旋转整个根节点(老行为)。
        // ChangePosition / ChangeRotation 两个处理器共用,保证一致:有 RootDir 时只转 RootDir(头像等其它层保持不动),没有时转根。
        public static void SyncRotation(GameObject go, quaternion rotation)
        {
            if (go == null)
            {
                return;
            }
            Transform dirTrans = FindDeepChild(go.transform, "RootDir");
            if (dirTrans != null)
            {
                dirTrans.rotation = rotation;
                return;
            }
            go.transform.rotation = rotation;
        }

        // 深度优先递归找指定名字的子节点(无 GC:不分配数组,逐层 GetChild)。
        private static Transform FindDeepChild(Transform parent, string name)
        {
            int count = parent.childCount;
            for (int i = 0; i < count; i++)
            {
                Transform child = parent.GetChild(i);
                if (child.name == name)
                {
                    return child;
                }
                Transform found = FindDeepChild(child, name);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }

        public static void OnTerrain(Transform transform)
        {
            // 平面玩法(球球大作战)进图时 GameObjectPosConfig.EnableTerrain=false:跳过贴地射线,
            // 让单位保持服务端 Y(=0,否则会被 Map 层几何贴到非0高度、食物甚至被推到背景下方而看不见),并省掉每次移动的射线开销。
            if (!GameObjectPosConfig.EnableTerrain)
            {
                return;
            }
            // 贴地
            Ray ray = new(new Vector3(transform.position.x, transform.position.y + 100, transform.position.z), Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 200, LayerMask.GetMask("Map")))
            {
                transform.position = hit.point;
            }
        }
    }
}