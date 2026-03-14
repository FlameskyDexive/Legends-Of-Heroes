using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [ComponentOf(typeof(DlgRoomList))]
    [EnableMethod]
    public class DlgRoomListViewComponent : Entity, IAwake, IDestroy
    {
        public UnityEngine.RectTransform EGBackGroundRectTransform
        {
            get
            {
                if (this.uiTransform == null)
                {
                    Log.Error("uiTransform is null.");
                    return null;
                }
                if (this.m_EGBackGroundRectTransform == null)
                {
                    this.m_EGBackGroundRectTransform = UIFindHelper.FindDeepChild<UnityEngine.RectTransform>(this.uiTransform.gameObject, "EGBackGround");
                }
                return this.m_EGBackGroundRectTransform;
            }
        }

        public UnityEngine.UI.Button ERefreshButton
        {
            get
            {
                if (this.uiTransform == null)
                {
                    Log.Error("uiTransform is null.");
                    return null;
                }
                if (this.m_ERefreshButton == null)
                {
                    this.m_ERefreshButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject, "EGBackGround/ERefresh");
                }
                return this.m_ERefreshButton;
            }
        }

        public UnityEngine.UI.Button ECreateRoomButton
        {
            get
            {
                if (this.uiTransform == null)
                {
                    Log.Error("uiTransform is null.");
                    return null;
                }
                if (this.m_ECreateRoomButton == null)
                {
                    this.m_ECreateRoomButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject, "EGBackGround/ECreateRoom");
                }
                return this.m_ECreateRoomButton;
            }
        }

        public UnityEngine.UI.Button EBackButton
        {
            get
            {
                if (this.uiTransform == null)
                {
                    Log.Error("uiTransform is null.");
                    return null;
                }
                if (this.m_EBackButton == null)
                {
                    this.m_EBackButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject, "EGBackGround/EBack");
                }
                return this.m_EBackButton;
            }
        }

        public UnityEngine.UI.ScrollRect ERoomListScrollRect
        {
            get
            {
                if (this.uiTransform == null)
                {
                    Log.Error("uiTransform is null.");
                    return null;
                }
                if (this.m_ERoomListScrollRect == null)
                {
                    this.m_ERoomListScrollRect = UIFindHelper.FindDeepChild<UnityEngine.UI.ScrollRect>(this.uiTransform.gameObject, "EGBackGround/ERoomList");
                }
                return this.m_ERoomListScrollRect;
            }
        }

        public UnityEngine.RectTransform ERoomListContentRectTransform
        {
            get
            {
                if (this.uiTransform == null)
                {
                    Log.Error("uiTransform is null.");
                    return null;
                }
                if (this.m_ERoomListContentRectTransform == null)
                {
                    this.m_ERoomListContentRectTransform = UIFindHelper.FindDeepChild<UnityEngine.RectTransform>(this.uiTransform.gameObject, "EGBackGround/ERoomList/Viewport/Content");
                }
                return this.m_ERoomListContentRectTransform;
            }
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

        private UnityEngine.RectTransform m_EGBackGroundRectTransform = null;
        private UnityEngine.UI.Button m_ERefreshButton = null;
        private UnityEngine.UI.Button m_ECreateRoomButton = null;
        private UnityEngine.UI.Button m_EBackButton = null;
        private UnityEngine.UI.ScrollRect m_ERoomListScrollRect = null;
        private UnityEngine.RectTransform m_ERoomListContentRectTransform = null;
        public Transform uiTransform = null;
    }
}