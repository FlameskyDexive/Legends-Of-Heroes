
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[EnableMethod]
	public partial class Scroll_Item_role : Entity,IAwake,IDestroy,IUIScrollItem 
	{
		public long DataId {get;set;}
		private bool isCacheNode = false;
		public void SetCacheMode(bool isCache)
		{
			this.isCacheNode = isCache;
		}

		public Scroll_Item_role BindTrans(Transform trans)
		{
			if (!Object.ReferenceEquals(this.uiTransform, trans) && !Object.ReferenceEquals(this.uiTransform, null))
			{
			}

			this.uiTransform = trans;
			return this;
		}

		public UnityEngine.UI.Image E_IconBgImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
			        this.Fiber().Error("uiTransform is null.");
     				return null;
     			}
     			if (this.isCacheNode)
     			{
     				if( this.m_E_IconBgImage == null )
     				{
		    			this.m_E_IconBgImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"E_IconBg");
     				}
     				return this.m_E_IconBgImage;
     			}
     			else
     			{
		    		return UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"E_IconBg");
     			}
     		}
     	}

		public UnityEngine.UI.Button E_AvatarButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
			        this.Fiber().Error("uiTransform is null.");
     				return null;
     			}
     			if (this.isCacheNode)
     			{
     				if( this.m_E_AvatarButton == null )
     				{
		    			this.m_E_AvatarButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"E_Avatar");
     				}
     				return this.m_E_AvatarButton;
     			}
     			else
     			{
		    		return UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"E_Avatar");
     			}
     		}
     	}

		public UnityEngine.UI.Image E_AvatarImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
			        this.Fiber().Error("uiTransform is null.");
     				return null;
     			}
     			if (this.isCacheNode)
     			{
     				if( this.m_E_AvatarImage == null )
     				{
		    			this.m_E_AvatarImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"E_Avatar");
     				}
     				return this.m_E_AvatarImage;
     			}
     			else
     			{
		    		return UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"E_Avatar");
     			}
     		}
     	}

		public UnityEngine.UI.Text E_RoleNameText
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
			        this.Fiber().Error("uiTransform is null.");
     				return null;
     			}
     			if (this.isCacheNode)
     			{
     				if( this.m_E_RoleNameText == null )
     				{
		    			this.m_E_RoleNameText = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"E_RoleName");
     				}
     				return this.m_E_RoleNameText;
     			}
     			else
     			{
		    		return UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"E_RoleName");
     			}
     		}
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

		private UnityEngine.UI.Image m_E_IconBgImage = null;
		private UnityEngine.UI.Button m_E_AvatarButton = null;
		private UnityEngine.UI.Image m_E_AvatarImage = null;
		private UnityEngine.UI.Text m_E_RoleNameText = null;
		public Transform uiTransform = null;
	}
}
