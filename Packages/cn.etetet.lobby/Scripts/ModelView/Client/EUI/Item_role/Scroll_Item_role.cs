using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [EnableMethod]
    public class Scroll_Item_role : Entity, IAwake, IDestroy, IUIScrollItem<Scroll_Item_role>
    {
        public long DataId { get; set; }
        public Transform uiTransform;

        public Scroll_Item_role BindTrans(Transform trans)
        {
            this.uiTransform = trans;
            return this;
        }

        public void SetCacheMode(bool isCache)
        {
            this.isCacheNode = isCache;
        }

        public Image E_IconBgImage => this.GetCachedChild<Image>(ref this.m_E_IconBgImage, "E_IconBg");
        public Button E_AvatarButton => this.GetCachedChild<Button>(ref this.m_E_AvatarButton, "E_Avatar");
        public Image E_AvatarImage => this.GetCachedChild<Image>(ref this.m_E_AvatarImage, "E_Avatar");
        public Text E_RoleNameText => this.GetCachedChild<Text>(ref this.m_E_RoleNameText, "E_RoleName");

        private T GetCachedChild<T>(ref T cache, string path) where T : Component
        {
            if (this.uiTransform == null) { Log.Error("uiTransform is null."); return null; }
            if (this.isCacheNode)
            {
                if (cache == null)
                {
                    cache = UIFindHelper.FindDeepChild<T>(this.uiTransform.gameObject, path);
                }
                return cache;
            }
            return UIFindHelper.FindDeepChild<T>(this.uiTransform.gameObject, path);
        }

        public void DestroyWidget()
        {
            this.m_E_IconBgImage = null;
            this.m_E_AvatarButton = null;
            this.m_E_AvatarImage = null;
            this.m_E_RoleNameText = null;
            this.uiTransform = null;
            this.DataId = 0;
        }

        private bool isCacheNode;
        private Image m_E_IconBgImage;
        private Button m_E_AvatarButton;
        private Image m_E_AvatarImage;
        private Text m_E_RoleNameText;
    }
}
