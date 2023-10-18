
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[ComponentOf(typeof(UIBaseWindow))]
	[EnableMethod]
	public  class DlgLobbyViewComponent : Entity,IAwake,IDestroy 
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
     			if( this.m_EGBackGroundRectTransform == null )
     			{
		    		this.m_EGBackGroundRectTransform = UIFindHelper.FindDeepChild<UnityEngine.RectTransform>(this.uiTransform.gameObject,"EGBackGround");
     			}
     			return this.m_EGBackGroundRectTransform;
     		}
     	}

		public UnityEngine.UI.Button E_EnterMapButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_EnterMapButton == null )
     			{
		    		this.m_E_EnterMapButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"EGBackGround/E_EnterMap");
     			}
     			return this.m_E_EnterMapButton;
     		}
     	}

		public UnityEngine.UI.Image E_EnterMapImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_EnterMapImage == null )
     			{
		    		this.m_E_EnterMapImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"EGBackGround/E_EnterMap");
     			}
     			return this.m_E_EnterMapImage;
     		}
     	}

		public ET.EUIButton E_AvatarEUIButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_AvatarEUIButton == null )
     			{
		    		this.m_E_AvatarEUIButton = UIFindHelper.FindDeepChild<ET.EUIButton>(this.uiTransform.gameObject,"EGBackGround/Top/PlayerInfo/E_Avatar");
     			}
     			return this.m_E_AvatarEUIButton;
     		}
     	}

		public ET.EUIImage E_AvatarEUIImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_AvatarEUIImage == null )
     			{
		    		this.m_E_AvatarEUIImage = UIFindHelper.FindDeepChild<ET.EUIImage>(this.uiTransform.gameObject,"EGBackGround/Top/PlayerInfo/E_Avatar");
     			}
     			return this.m_E_AvatarEUIImage;
     		}
     	}

		public UnityEngine.UI.Text E_PlayerNameText
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_PlayerNameText == null )
     			{
		    		this.m_E_PlayerNameText = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"EGBackGround/Top/PlayerInfo/E_PlayerName");
     			}
     			return this.m_E_PlayerNameText;
     		}
     	}

		public UnityEngine.UI.Button E_SoloButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_SoloButton == null )
     			{
		    		this.m_E_SoloButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"EGBackGround/E_Solo");
     			}
     			return this.m_E_SoloButton;
     		}
     	}

		public UnityEngine.UI.Image E_SoloImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_SoloImage == null )
     			{
		    		this.m_E_SoloImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"EGBackGround/E_Solo");
     			}
     			return this.m_E_SoloImage;
     		}
     	}

		public UnityEngine.UI.Button E_BackToLoginButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_BackToLoginButton == null )
     			{
		    		this.m_E_BackToLoginButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"EGBackGround/E_BackToLogin");
     			}
     			return this.m_E_BackToLoginButton;
     		}
     	}

		public UnityEngine.UI.Image E_BackToLoginImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_BackToLoginImage == null )
     			{
		    		this.m_E_BackToLoginImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"EGBackGround/E_BackToLogin");
     			}
     			return this.m_E_BackToLoginImage;
     		}
     	}

		public void DestroyWidget()
		{
			this.m_EGBackGroundRectTransform = null;
			this.m_E_EnterMapButton = null;
			this.m_E_EnterMapImage = null;
			this.m_E_AvatarEUIButton = null;
			this.m_E_AvatarEUIImage = null;
			this.m_E_PlayerNameText = null;
			this.m_E_SoloButton = null;
			this.m_E_SoloImage = null;
			this.m_E_BackToLoginButton = null;
			this.m_E_BackToLoginImage = null;
			this.uiTransform = null;
		}

		private UnityEngine.RectTransform m_EGBackGroundRectTransform = null;
		private UnityEngine.UI.Button m_E_EnterMapButton = null;
		private UnityEngine.UI.Image m_E_EnterMapImage = null;
		private ET.EUIButton m_E_AvatarEUIButton = null;
		private ET.EUIImage m_E_AvatarEUIImage = null;
		private UnityEngine.UI.Text m_E_PlayerNameText = null;
		private UnityEngine.UI.Button m_E_SoloButton = null;
		private UnityEngine.UI.Image m_E_SoloImage = null;
		private UnityEngine.UI.Button m_E_BackToLoginButton = null;
		private UnityEngine.UI.Image m_E_BackToLoginImage = null;
		public Transform uiTransform = null;
	}
}
