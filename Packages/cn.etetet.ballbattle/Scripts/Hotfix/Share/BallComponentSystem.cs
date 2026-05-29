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
    }
}
