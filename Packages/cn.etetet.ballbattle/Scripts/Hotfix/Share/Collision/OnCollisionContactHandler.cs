namespace ET
{
    // 碰撞裁决:由 Box2D BeginContact → OnCollisionContact 事件触发。服务端权威。
    // Box2D 接触只触发一次且 A/B 顺序不定,故每种组合判两个朝向。
    [Event(SceneType.Map)]
    public class OnCollisionContactHandler : AEvent<Scene, OnCollisionContact>
    {
        protected override async ETTask Run(Scene scene, OnCollisionContact args)
        {
            Unit unitA = args.contact.FixtureA.UserData as Unit;
            Unit unitB = args.contact.FixtureB.UserData as Unit;
            if (unitA == null || unitB == null || unitA.IsDisposed || unitB.IsDisposed)
            {
                return;
            }

            EBallType ta = unitA.GetBallType();
            EBallType tb = unitB.GetBallType();

            // 子弹命中玩家(两个朝向)
            if (ta == EBallType.Bullet && tb == EBallType.Player)
            {
                BulletHit(target: unitB, bullet: unitA);
            }
            else if (tb == EBallType.Bullet && ta == EBallType.Player)
            {
                BulletHit(target: unitA, bullet: unitB);
            }
            // 玩家吃食物(两个朝向)
            else if (ta == EBallType.Player && tb == EBallType.Food)
            {
                Devour(eater: unitA, eaten: unitB);
            }
            else if (tb == EBallType.Player && ta == EBallType.Food)
            {
                Devour(eater: unitB, eaten: unitA);
            }
            // 玩家吃玩家:比半径,大吃小
            else if (ta == EBallType.Player && tb == EBallType.Player)
            {
                ResolvePvp(unitA, unitB);
            }

            await ETTask.CompletedTask;
        }

        // 大球吞小球:被吃方质量(HP)转给吃方,移除被吃方
        private static void Devour(Unit eater, Unit eaten)
        {
            NumericComponent eaterNc = eater.NumericComponent;
            NumericComponent eatenNc = eaten.NumericComponent;
            if (eaterNc == null || eatenNc == null)
            {
                return;
            }

            int gain = eatenNc.GetAsInt(NumericType.HP);
            eaterNc.Set(NumericType.HP, eaterNc.GetAsInt(NumericType.HP) + gain);
            eater.RecalcFromHp(); // 变大 + 广播

            eaten.GetParent<UnitComponent>().Remove(eaten.Id); // AOI 自动发 M2C_RemoveUnits
        }

        // 玩家间:半径明显更大者吞噬对方,体型相近不处理
        private static void ResolvePvp(Unit a, Unit b)
        {
            float ra = a.GetRadius();
            float rb = b.GetRadius();
            if (ra > rb * BallDefine.EatRatio)
            {
                Devour(a, b);
            }
            else if (rb > ra * BallDefine.EatRatio)
            {
                Devour(b, a);
            }
        }

        // 子弹命中:扣血→变小,死亡则移除;子弹消失
        private static void BulletHit(Unit target, Unit bullet)
        {
            NumericComponent nc = target.NumericComponent;
            if (nc != null)
            {
                int newHp = nc.GetAsInt(NumericType.HP) - BallDefine.BulletDamage;
                nc.Set(NumericType.HP, newHp < 0 ? 0 : newHp);
                target.RecalcFromHp(); // 变小 + 广播

                if (newHp <= 0)
                {
                    // TODO: 死亡事件(结算/重生),先直接移除
                    target.GetParent<UnitComponent>().Remove(target.Id);
                }
            }

            bullet.GetParent<UnitComponent>().Remove(bullet.Id);
        }
    }
}
