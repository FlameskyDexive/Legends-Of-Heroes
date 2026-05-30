using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    // 预制体控件容器(懒加载)。节点路径与 DlgBallLeaderboard.prefab 命名一致:
    // 根 DlgBallLeaderboard 下 EGBg(底板) → ETitle(标题 Text) / EContent(多行内容 Text) / EClose(关闭 Button)。
    [ComponentOf(typeof(DlgBallLeaderboard))]
    [EnableMethod]
    public class DlgBallLeaderboardViewComponent : Entity, IAwake, IDestroy
    {
        public Transform uiTransform;

        public Button ECloseButton => this.GetChild<Button>(ref this.m_ECloseButton, "EGBg/EClose");
        public Text ETitleText => this.GetChild<Text>(ref this.m_ETitleText, "EGBg/ETitle");
        // 单个多行 Text 承载 Top5 全部行(每 5s 刷新一次)
        public Text EContentText => this.GetChild<Text>(ref this.m_EContentText, "EGBg/EContent");

        private T GetChild<T>(ref T cache, string path) where T : Component
        {
            if (this.uiTransform == null)
            {
                Log.Error("uiTransform is null.");
                return null;
            }
            if (cache == null)
            {
                cache = UIFindHelper.FindDeepChild<T>(this.uiTransform.gameObject, path);
            }
            return cache;
        }

        public void DestroyWidget()
        {
            this.m_ECloseButton = null;
            this.m_ETitleText = null;
            this.m_EContentText = null;
            this.uiTransform = null;
        }

        private Button m_ECloseButton;
        private Text m_ETitleText;
        private Text m_EContentText;
    }
}
