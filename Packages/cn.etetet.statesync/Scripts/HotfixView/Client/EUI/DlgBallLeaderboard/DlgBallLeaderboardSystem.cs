using System.Collections.Generic;
using System.Text;

namespace ET.Client
{
    [FriendOf(typeof(DlgBallLeaderboard))]
    public static class DlgBallLeaderboardSystem
    {
        public static void RegisterUIEvent(this DlgBallLeaderboard self)
        {
            EntityRef<DlgBallLeaderboard> selfRef = self;
            // 关闭按钮可能在预制体里缺失,判空兜底
            if (self.View.ECloseButton != null)
            {
                self.View.ECloseButton.AddListener(self.Root(), () => OnCloseClick(selfRef));
            }
            if (self.View.ETitleText != null)
            {
                self.View.ETitleText.text = "排行榜";
            }
        }

        public static void ShowWindow(this DlgBallLeaderboard self, Entity contextData = null)
        {
            // 打开时先清空,等服务端广播刷新
            self.RefreshRank(null);
        }

        // 用服务端广播的排行数据刷新内容(单个多行 Text;ranks 为 null/空则显示提示)。
        public static void RefreshRank(this DlgBallLeaderboard self, List<BallRankInfo> ranks)
        {
            UnityEngine.UI.Text content = self.View.EContentText;
            if (content == null)
            {
                return;
            }
            if (ranks == null || ranks.Count == 0)
            {
                content.text = "暂无数据";
                return;
            }

            StringBuilder sb = new StringBuilder();
            int count = ranks.Count < DlgBallLeaderboard.RowCount ? ranks.Count : DlgBallLeaderboard.RowCount;
            for (int i = 0; i < count; i++)
            {
                BallRankInfo r = ranks[i];
                sb.AppendLine($"#{i + 1}  玩家{r.UnitId % 100000}  HP {r.Hp}  击杀 {r.Kills}");
            }
            content.text = sb.ToString();
        }

        private static void OnCloseClick(EntityRef<DlgBallLeaderboard> selfRef)
        {
            DlgBallLeaderboard self = selfRef;
            if (self == null)
            {
                return;
            }
            self.Root().GetComponent<UIComponent>().HideWindow(WindowID.WindowID_BallLeaderboard);
        }
    }
}
