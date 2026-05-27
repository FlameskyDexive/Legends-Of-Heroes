using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [ComponentOf(typeof(DlgRoomList))]
    [EnableMethod]
    public class DlgRoomListViewComponent : Entity, IAwake, IDestroy
    {
        public Transform uiTransform;

        public RectTransform EGBackGroundRectTransform => this.GetChild<RectTransform>(ref this.m_EGBackGroundRectTransform, "EGBackGround");
        public Button ERefreshButton => this.GetChild<Button>(ref this.m_ERefreshButton, "EGBackGround/ERefresh");
        public Button ECreateRoomButton => this.GetChild<Button>(ref this.m_ECreateRoomButton, "EGBackGround/ECreateRoom");
        public Button EBackButton => this.GetChild<Button>(ref this.m_EBackButton, "EGBackGround/EBack");
        public ScrollRect ERoomListScrollRect => this.GetChild<ScrollRect>(ref this.m_ERoomListScrollRect, "EGBackGround/ERoomList");
        public RectTransform ERoomListContentRectTransform => this.GetChild<RectTransform>(ref this.m_ERoomListContentRectTransform, "EGBackGround/ERoomList/Viewport/Content");

        private T GetChild<T>(ref T cache, string path) where T : Component
        {
            if (this.uiTransform == null) { Log.Error("uiTransform is null."); return null; }
            if (cache == null)
            {
                cache = UIFindHelper.FindDeepChild<T>(this.uiTransform.gameObject, path);
            }
            return cache;
        }

        public void DestroyWidget()
        {
            this.m_EGBackGroundRectTransform = null;
            this.m_ERefreshButton = null;
            this.m_ECreateRoomButton = null;
            this.m_EBackButton = null;
            this.m_ERoomListScrollRect = null;
            this.m_ERoomListContentRectTransform = null;
            this.uiTransform = null;
        }

        private RectTransform m_EGBackGroundRectTransform;
        private Button m_ERefreshButton;
        private Button m_ECreateRoomButton;
        private Button m_EBackButton;
        private ScrollRect m_ERoomListScrollRect;
        private RectTransform m_ERoomListContentRectTransform;
    }
}
