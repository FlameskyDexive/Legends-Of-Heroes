using UnityEngine;

namespace ET.Client
{
    // 客户端:Radius 变化时缩放球的视图 GameObject,实现"血量变→大小变"的表现。
    // 体型来自服务端权威广播(M2C_NumericChange),这里只做表现。
    [NumericWatcher(SceneType.Current, NumericType.Radius)]
    public class NumericWatcher_Radius_ScaleBall : INumericWatcher
    {
        public void Run(Unit unit, NumbericChange args)
        {
            GameObjectComponent goComponent = unit.GetComponent<GameObjectComponent>();
            GameObject go = goComponent?.GameObject;
            if (go == null)
            {
                return;
            }

            float radius = args.New / 1000f;       // 还原定点
            float diameter = radius * 2f;            // 视觉直径(按需调系数)
            go.transform.localScale = new Vector3(diameter, diameter, diameter);
        }
    }
}
