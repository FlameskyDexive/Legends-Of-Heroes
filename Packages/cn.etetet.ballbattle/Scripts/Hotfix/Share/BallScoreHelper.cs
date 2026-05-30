using System.Collections.Generic;

namespace ET
{
    // 球球计分 / 结算(服务端权威):击杀-死亡统计 + 服务端排行榜日志。
    // 计分挂在 BallComponent(Kills/Deaths);子弹击杀经子弹的 ShooterId 反查射手记功。
    // 注:客户端排行榜 UI / M2C 广播为后续(需加 proto 消息),当前先用服务端日志观察。
    [FriendOf(typeof(BallComponent))]
    public static class BallScoreHelper
    {
        // 记一次击杀:victim 死亡数+1;killer 若为玩家球(且非自杀)则击杀数+1, 并打一条击杀日志。
        public static void CreditKill(Unit killer, Unit victim)
        {
            if (victim == null)
            {
                return;
            }

            BallComponent vc = victim.GetComponent<BallComponent>();
            if (vc != null)
            {
                vc.Deaths++;
            }

            BallComponent kc = killer?.GetComponent<BallComponent>();
            if (kc != null && kc.BallType == EBallType.Player && killer.Id != victim.Id)
            {
                kc.Kills++;
                Log.Info($"[球球击杀] Unit {killer.Id} 吃掉 Unit {victim.Id} (累计击杀 {kc.Kills})");
            }
        }

        // 子弹击杀:从子弹的 ShooterId 反查射手玩家再记功(射手已离场则只记被击杀方死亡)。
        public static void CreditBulletKill(Unit bullet, Unit victim)
        {
            if (bullet == null || victim == null)
            {
                return;
            }

            BallComponent bc = bullet.GetComponent<BallComponent>();
            long shooterId = bc != null ? bc.ShooterId : 0;
            Unit shooter = null;
            if (shooterId != 0)
            {
                UnitComponent uc = victim.Scene().GetComponent<UnitComponent>();
                if (uc != null && uc.Children.TryGetValue(shooterId, out Entity e))
                {
                    shooter = e as Unit;
                }
            }
            CreditKill(shooter, victim);
        }

        // 取场景内玩家球, 按当前 HP(体型)降序填入 result(最多 topN)。供排行榜广播/日志复用。
        public static void GatherTopPlayers(Scene scene, int topN, List<Unit> result)
        {
            result.Clear();
            UnitComponent uc = scene?.GetComponent<UnitComponent>();
            if (uc == null)
            {
                return;
            }
            foreach (Entity child in uc.Children.Values)
            {
                if (child is Unit u && !u.IsDisposed && u.GetBallType() == EBallType.Player)
                {
                    result.Add(u);
                }
            }
            result.Sort((a, b) => b.NumericComponent.GetAsInt(NumericType.HP) - a.NumericComponent.GetAsInt(NumericType.HP));
            if (result.Count > topN)
            {
                result.RemoveRange(topN, result.Count - topN);
            }
        }

        // 读一个球的累计击杀(无 BallComponent 返回 0)
        public static int GetKills(Unit unit)
        {
            BallComponent bc = unit?.GetComponent<BallComponent>();
            return bc != null ? bc.Kills : 0;
        }
    }
}
