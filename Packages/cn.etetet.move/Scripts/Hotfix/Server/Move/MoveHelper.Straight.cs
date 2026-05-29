using System.Collections.Generic;
using Unity.Mathematics;

namespace ET.Server
{
    // 直线移动(不走 Recast 寻路):用于开阔无障碍场景(如球球大作战竞技场)。
    // 玩家单位通常没有 PathfindingComponent,不能用 FindPathMoveToAsync,这里直接构造两点路径。
    public static partial class MoveHelper
    {
        public static async ETTask StraightMoveToAsync(this Unit unit, float3 target, int turnTime = 100)
        {
            float speed = unit.NumericComponent.GetAsFloat(NumericType.Speed);
            if (speed < 0.01f)
            {
                unit.SendStop(2);
                return;
            }

            // 两点直线路径:当前位置 → 目标
            M2C_PathfindingResult m2CPathfindingResult = M2C_PathfindingResult.Create();
            m2CPathfindingResult.Points.Add(unit.Position);
            m2CPathfindingResult.Points.Add(target);
            m2CPathfindingResult.Id = unit.Id;

            // 广播路径,客户端插值跟随
            MapMessageHelper.NoticeClient(unit, m2CPathfindingResult, NoticeType.Broadcast);

            MoveComponent moveComponent = unit.GetComponent<MoveComponent>();
            EntityRef<Unit> unitRef = unit;

            bool ret = await moveComponent.MoveToAsync(m2CPathfindingResult.Points, speed, turnTime);
            if (ret) // false 表示被其它移动取消,不需通知 stop
            {
                unit = unitRef;
                unit.SendStop(0);
            }
        }
    }
}
