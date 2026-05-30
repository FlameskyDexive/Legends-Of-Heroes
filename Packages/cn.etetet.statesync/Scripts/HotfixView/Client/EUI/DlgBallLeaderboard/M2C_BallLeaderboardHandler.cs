namespace ET.Client
{
    // 收到服务端排行榜广播:确保排行榜弹窗已打开,并用最新数据刷新。
    [MessageHandler(SceneType.Current)]
    public class M2C_BallLeaderboardHandler : MessageHandler<Scene, M2C_BallLeaderboard>
    {
        protected override async ETTask Run(Scene scene, M2C_BallLeaderboard message)
        {
            UIComponent ui = scene.Root().GetComponent<UIComponent>();
            if (ui != null)
            {
                if (!ui.IsWindowVisible(WindowID.WindowID_BallLeaderboard))
                {
                    ui.ShowWindow<DlgBallLeaderboard>();
                }
                DlgBallLeaderboard dlg = ui.GetDlgLogic<DlgBallLeaderboard>(true);
                dlg?.RefreshRank(message.Ranks);
            }
            await ETTask.CompletedTask;
        }
    }
}
