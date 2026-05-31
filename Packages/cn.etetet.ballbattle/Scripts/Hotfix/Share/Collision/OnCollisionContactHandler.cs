namespace ET
{
    // 碰撞裁决:由 Box2D BeginContact → OnCollisionContact 事件触发。服务端权威。
    // Box2D 接触只触发一次且 A/B 顺序不定,故每种组合判两个朝向。
    [Event(SceneType.Map)]
    public class OnCollisionContactHandler : AEvent<Scene, OnCollisionContact>
    {
        protected override async ETTask Run(Scene scene, OnCollisionContact args)
        {
            // 事件已在 World.Step 之后发(携带单位 id)。按 id 重新取单位:同一帧前一对接触的裁决可能已把某单位吃掉/移除,
            // 故必须判空/判销毁(不再用 Box2D Contact 的 UserData,那在 Step 后可能失效)。
            UnitComponent unitComponent = scene.GetComponent<UnitComponent>();
            if (unitComponent == null)
            {
                return;
            }
            Unit unitA = unitComponent.Children.TryGetValue(args.UnitIdA, out Entity ea) ? ea as Unit : null;
            Unit unitB = unitComponent.Children.TryGetValue(args.UnitIdB, out Entity eb) ? eb as Unit : null;
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

            if (eaten.GetBallType() == EBallType.Player)
            {
                // 计分:吞噬方记一次击杀, 被吞方记一次死亡(结算/排行榜)
                BallScoreHelper.CreditKill(eater, eaten);
                // 玩家被吞: 触发死亡 -> 重生(服务端订阅 BallPlayerDie), 不直接移除(保持连接)
                EventSystem.Instance.Publish(eaten.Scene(), new BallPlayerDie { Dead = eaten, Killer = eater });
            }
            else
            {
                eaten.GetParent<UnitComponent>().Remove(eaten.Id); // 食物/子弹: AOI 自动发 M2C_RemoveUnits
            }
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
                    // 计分:经子弹 ShooterId 反查射手记一次击杀, 被击杀方记死亡(在移除子弹前取 ShooterId)
                    BallScoreHelper.CreditBulletKill(bullet, target);
                    // 玩家被子弹击杀: 触发死亡 -> 重生(服务端订阅), 不直接移除
                    EventSystem.Instance.Publish(target.Scene(), new BallPlayerDie { Dead = target, Killer = bullet });
                }
            }

            bullet.GetParent<UnitComponent>().Remove(bullet.Id);
        }
    }
}
