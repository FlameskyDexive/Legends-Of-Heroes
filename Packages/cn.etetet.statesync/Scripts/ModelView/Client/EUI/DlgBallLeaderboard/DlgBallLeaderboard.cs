namespace ET.Client
{
    // 球球大作战排行榜弹窗(PopUp)。固定 Top5 行,数据由服务端 M2C_BallLeaderboard 广播驱动刷新。
    [ComponentOf(typeof(UIBaseWindow))]
    public class DlgBallLeaderboard : Entity, IAwake, IUILogic
    {
        public DlgBallLeaderboardViewComponent View => this.GetComponent<DlgBallLeaderboardViewComponent>();

        // 固定行数(预制体里有 ERow0..ERow4 五个 Text)
        public const int RowCount = 5;
    }
}
