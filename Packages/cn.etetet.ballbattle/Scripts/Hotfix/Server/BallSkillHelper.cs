using Unity.Mathematics;

namespace ET.Server
{
    // 球球技能的服务端实现:吐球(喷射小子弹)/分裂(喷出半质量冲刺球)。
    // 由 skill 时间轴的 ActionEvent 触发(ActionEventBallSpit/Split),用 ballbattle 自己的球逻辑实现。
    public static class BallSkillHelper
    {
        // 喷出一个 Bullet 类型的球, 朝 owner 朝向飞行 range 距离后移除。
        public static async ETTask EjectBall(this Unit owner, int ejectHp, int costHp, float range)
        {
            if (owner == null || owner.IsDisposed)
            {
                return;
            }

            Scene scene = owner.Scene();
            NumericComponent nc = owner.NumericComponent;
            int hp = nc.GetAsInt(NumericType.HP);
            if (hp - costHp < (int)BallDefine.MinHp)
            {
                return; // 太小, 喷不出
            }

            float3 dir = owner.Forward;
            dir.y = 0;
            if (math.lengthsq(dir) < 0.0001f)
            {
                dir = new float3(0, 0, 1);
            }
            dir = math.normalize(dir);

            nc.Set(NumericType.HP, hp - costHp);
            owner.RecalcFromHp();

            float spawnDist = owner.NumericComponent.GetAsFloat(NumericType.Radius) + 2.5f;
            long id = IdGenerater.Instance.GenerateId();
            Unit bullet = UnitFactory.Create(scene, id, BallDefine.BulletConfigId);
            bullet.Position = owner.Position + dir * spawnDist;
            bullet.NumericComponent.Set(NumericType.HP, ejectHp);
            bullet.SetupBall(EBallType.Bullet);
            bullet.SetShooter(owner.Id); // 记录射手, 子弹击杀玩家时给射手记功

            float3 target = bullet.Position + dir * range;
            EntityRef<Unit> bulletRef = bullet;
            await bullet.StraightMoveToAsync(target);

            bullet = bulletRef;
            if (bullet != null && !bullet.IsDisposed)
            {
                bullet.GetParent<UnitComponent>().Remove(bullet.Id);
            }
        }

        // 吐球:喷小子弹(参数来自 skill 多态配置 ActionEventParams_BallSpit)
        public static void SpitBall(this Unit owner, int costHp, int bulletHp, float range)
        {
            owner.EjectBall(bulletHp, costHp, range).Coroutine();
        }

        // 分裂:喷出自身一半 HP 的冲刺球(参数来自 skill 多态配置 ActionEventParams_BallSplit)
        public static void SplitBall(this Unit owner, int minHp, float range)
        {
            if (owner == null || owner.IsDisposed)
            {
                return;
            }
            int hp = owner.NumericComponent.GetAsInt(NumericType.HP);
            if (hp < minHp)
            {
                return;
            }
            int half = hp / 2;
            owner.EjectBall(half, half, range).Coroutine();
        }
    }
}
