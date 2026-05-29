using Unity.Mathematics;

namespace ET.Server
{
    // 客户端每帧上报的操作批处理(摇杆移动 / 技能按键)。逐条分发:
    // - Move + 按下/持续: 朝摇杆方向寻路移动; Move + 抬起: 停止移动。
    // - Skill1/Skill2: 发布 OperateSkillCast 事件, 由战斗包(cn.etetet.skill)订阅并释放技能,
    //   避免 mapplay(低层) 反向依赖 skill(高层)。
    [MessageHandler(SceneType.Map)]
    public class C2M_OperationHandler : MessageLocationHandler<Unit, C2M_Operation>
    {
        // 摇杆方向仅给方向, 取单位前方一段距离作为寻路目标(每帧重发, 取较短距离即可)。
        private const float MoveDistance = 5f;

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

                        float3 target = unit.Position + dir * MoveDistance;
                        unit.FindPathMoveToAsync(target).Coroutine();
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
