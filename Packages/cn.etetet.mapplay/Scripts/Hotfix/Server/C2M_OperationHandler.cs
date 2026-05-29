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
        // 取较远前瞻 + 同方向节流: 摇杆稳定时不重发路径, 避免 20 人同图每帧广播刷屏(速度式移动)。
        private const float MoveLookahead = 50f;

        // 方向夹角阈值(点积): 大于此值视为"同方向", 移动中则跳过重发。约 2.5°。
        private const float SameDirDot = 0.999f;

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

                        // 速度式移动: 仍在移动且方向几乎未变时, 不重发路径(否则每帧重广播, 多人同图刷屏)。
                        MoveComponent moveComponent = unit.GetComponent<MoveComponent>();
                        if (moveComponent != null && !moveComponent.IsArrived())
                        {
                            float3 fwd = unit.Forward;
                            fwd.y = 0;
                            if (math.lengthsq(fwd) > 0.0001f && math.dot(math.normalize(fwd), dir) > SameDirDot)
                            {
                                break; // 同方向继续滑行, 无需重发
                            }
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
