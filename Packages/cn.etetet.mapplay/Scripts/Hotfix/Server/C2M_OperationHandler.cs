using Unity.Mathematics;

namespace ET.Server
{
    // 客户端每帧上报的操作批处理(摇杆移动 / 技能按键)。逐条分发:
    // - Move + 按下/持续: 朝摇杆方向直线移动(开阔竞技场不走 Recast); Move + 抬起: 停止。
    // - Skill1/Skill2: 发布 OperateSkillCast 事件, 由战斗包(cn.etetet.skill)订阅并释放技能,
    //   避免 mapplay(低层) 反向依赖 skill(高层)。
    [MessageHandler(SceneType.Map)]
    public class C2M_OperationHandler : MessageLocationHandler<Unit, C2M_Operation>
    {
        // 摇杆只给方向, 取前方一段较远距离作为直线移动目标; 摇杆方向变化时重发即可持续转向。
        private const float MoveLookahead = 20f;

        protected override async ETTask Run(Unit unit, C2M_Operation message)
        {
            foreach (OperateInfo info in message.OperateInfos)
            {
                switch ((EOperateType)info.OperateType)
                {
                    case EOperateType.Move:
                    {
                        if ((EInputType)info.InputType == EInputType.KeyUp)
                        {
                            unit.Stop(1);
                            break;
                        }

                        float3 dir = math.normalizesafe(info.Vec3);
                        if (math.lengthsq(dir) < 0.0001f)
                        {
                            break;
                        }

                        // 直线移动(玩家球无 PathfindingComponent,不能用 Recast 寻路)
                        float3 target = unit.Position + dir * MoveLookahead;
                        unit.StraightMoveToAsync(target).Coroutine();
                        break;
                    }
                    case EOperateType.Skill1:
                        EventSystem.Instance.Publish(unit.Scene(), new OperateSkillCast { Unit = unit, Index = 0 });
                        break;
                    case EOperateType.Skill2:
                        EventSystem.Instance.Publish(unit.Scene(), new OperateSkillCast { Unit = unit, Index = 1 });
                        break;
                }
            }

            await ETTask.CompletedTask;
        }
    }
}
