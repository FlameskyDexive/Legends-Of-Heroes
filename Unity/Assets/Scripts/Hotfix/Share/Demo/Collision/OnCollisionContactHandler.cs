
using Box2DSharp.Dynamics.Contacts;

namespace ET
{
    [Event(SceneType.Map)]
    public class OnCollisionContactHandler: AEvent<Scene, EventType.OnCollisionContact>
    {
        protected override async ETTask Run(Scene scene, EventType.OnCollisionContact args)
        {
            Unit unitA = (Unit)args.contact.FixtureA.UserData;
            Unit unitB = (Unit)args.contact.FixtureB.UserData;
            if (unitA.IsDisposed || unitB.IsDisposed)
            {
                return;
            }
            Log.Info($"start contact:{unitA.Config?.Name}, {unitB.Config?.Name}");
            
            //当前子弹只处理子弹伤害，子弹回血（给队友回血/技能吸血自行拓展）
            if (unitA.Type == UnitType.Bullet && unitB.Type == UnitType.Player)
            {
                BattleHelper.HitSettle(unitA.GetComponent<BulletComponent>().OwnerUnit, unitB, EHitFromType.Skill_Bullet, unitA);
            }//由于box2d没有双向碰撞响应，处理不同类型的时候判断各自类型
            else if (unitA.Type == UnitType.Player && unitB.Type == UnitType.Bullet)
            {
                BattleHelper.HitSettle(unitA, unitB.GetComponent<BulletComponent>().OwnerUnit, EHitFromType.Skill_Bullet, unitB);
            }//玩家跟玩家碰撞，判定玩家重量大小，大吃小
            else if(unitA.Type == UnitType.Player && unitB.Type == UnitType.Player)
            {
                
            }//玩家吃到食物
            else if(unitA.Type == UnitType.Player && unitB.Type == UnitType.Food)
            {
                //获取食物的分量，添加给玩家，同时销毁食物单位
            }

            await ETTask.CompletedTask;
        }
    }
}