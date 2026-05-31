using System;
using Unity.Mathematics;

namespace ET.Server
{
    // 机器人球 AI(服务端权威):每 RobotThinkMs 思考一次——
    // 视野(同地图全部单位)里找最近的"猎物"(食物 / 比自己明显小的玩家球)去追,
    // 找最近的"威胁"(比自己明显大的球)在近距离时逃跑,否则随机游走;朝猎物偶尔吐球。
    // 直接用服务端移动(StraightMoveToAsync)驱动, 不经客户端输入, 等价于"服务端机器人模拟玩家"。
    [EntitySystemOf(typeof(RobotBallAIComponent))]
    [FriendOf(typeof(RobotBallAIComponent))]
    public static partial class RobotBallAIComponentSystem
    {
        [EntitySystem]
        public static void Awake(this RobotBallAIComponent self)
        {
            self.ThinkTimer = self.Root().TimerComponent.NewRepeatedTimer(BallDefine.RobotThinkMs, TimerInvokeType.BallRobotThink, self);
        }

        [EntitySystem]
        public static void Destroy(this RobotBallAIComponent self)
        {
            self.Root().TimerComponent?.Remove(ref self.ThinkTimer);
        }

        public static void Think(this RobotBallAIComponent self)
        {
            Unit me = self.GetParent<Unit>();
            if (me == null || me.IsDisposed)
            {
                return;
            }

            Scene scene = me.Scene();
            UnitComponent unitComponent = scene.GetComponent<UnitComponent>();
            if (unitComponent == null)
            {
                return;
            }

            float3 myPos = me.Position;
            float myRadius = me.GetRadius();
            if (myRadius < 0.01f)
            {
                return;
            }

            Unit prey = null;
            float preyDist = float.MaxValue;
            Unit threat = null;
            float threatDist = float.MaxValue;

            foreach (Entity child in unitComponent.Children.Values)
            {
                Unit u = child as Unit;
                if (u == null || u.Id == me.Id || u.IsDisposed)
                {
                    continue;
                }

                EBallType t = u.GetBallType();
                if (t == EBallType.None || t == EBallType.Bullet)
                {
                    continue;
                }

                float3 op = u.Position;
                float d = math.distance(myPos, op);
                float oR = u.GetRadius();

                if (oR > myRadius * BallDefine.EatRatio)
                {
                    if (d < threatDist)
                    {
                        threatDist = d;
                        threat = u;
                    }
                }
                else if (t == EBallType.Food || myRadius > oR * BallDefine.EatRatio)
                {
                    if (d < preyDist)
                    {
                        preyDist = d;
                        prey = u;
                    }
                }
            }

            float3 dir;
            if (threat != null && threatDist < BallDefine.RobotFleeRange)
            {
                dir = myPos - threat.Position; // 逃离威胁
            }
            else if (prey != null)
            {
                dir = prey.Position - myPos; // 追逐猎物
                // 机器人不再自动吐球:吐出的子弹用的是食物预制体(1102),会被误看成"会动的食物"。
                // 按需求,只有玩家角色用技能才吐食物;机器人只追逐/逃跑/游走 + 吃。
            }
            else
            {
                float ang = RandomGenerator.RandFloat01() * (2f * math.PI); // 随机游走
                dir = new float3(math.cos(ang), 0, math.sin(ang));
            }

            dir.y = 0;
            if (math.lengthsq(dir) < 0.0001f)
            {
                return;
            }
            dir = math.normalize(dir);
            float3 moveTarget = myPos + dir * BallDefine.RobotLookahead;
            // 同玩家:clamp 移动目标到地图边界,机器人不会走出地面。
            MapBoundsComponent bounds = scene.GetComponent<MapBoundsComponent>();
            if (bounds != null)
            {
                moveTarget.x = math.clamp(moveTarget.x, bounds.Min, bounds.Max);
                moveTarget.z = math.clamp(moveTarget.z, bounds.Min, bounds.Max);
            }
            me.StraightMoveToAsync(moveTarget).Coroutine();
        }
    }

    // 机器人思考定时器
    [Invoke(TimerInvokeType.BallRobotThink)]
    public class BallRobotThinkTimer : ATimer<RobotBallAIComponent>
    {
        protected override void Run(RobotBallAIComponent self)
        {
            try
            {
                self.Think();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
