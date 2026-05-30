namespace ET
{
    [EntitySystemOf(typeof(BallComponent))]
    [FriendOf(typeof(BallComponent))]
    public static partial class BallComponentSystem
    {
        [EntitySystem]
        public static void Awake(this BallComponent self, EBallType ballType)
        {
            self.BallType = ballType;
        }

        public static EBallType GetBallType(this Unit unit)
        {
            BallComponent ball = unit.GetComponent<BallComponent>();
            return ball == null ? EBallType.None : ball.BallType;
        }

        // 记录子弹的射手(发射者 UnitId), 用于子弹击杀记功
        public static void SetShooter(this Unit unit, long shooterId)
        {
            BallComponent ball = unit.GetComponent<BallComponent>();
            if (ball != null)
            {
                ball.ShooterId = shooterId;
            }
        }
    }
}
