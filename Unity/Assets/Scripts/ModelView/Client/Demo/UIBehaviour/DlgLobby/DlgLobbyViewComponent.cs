
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[ComponentOf(typeof(DlgLobby))]
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

		public UnityEngine.UI.Button EEnterMapButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EEnterMapButton == null )
     			{
		    		this.m_EEnterMapButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"EGBackGround/EEnterMap");
     			}
     			return this.m_EEnterMapButton;
     		}
     	}

		public UnityEngine.UI.Image EEnterMapImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EEnterMapImage == null )
     			{
		    		this.m_EEnterMapImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"EGBackGround/EEnterMap");
     			}
     			return this.m_EEnterMapImage;
     		}
     	}

		public ET.EUIButton EAvatarEUIButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EAvatarEUIButton == null )
     			{
		    		this.m_EAvatarEUIButton = UIFindHelper.FindDeepChild<ET.EUIButton>(this.uiTransform.gameObject,"EGBackGround/Top/PlayerInfo/EAvatar");
     			}
     			return this.m_EAvatarEUIButton;
     		}
     	}

		public ET.EUIImage EAvatarEUIImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EAvatarEUIImage == null )
     			{
		    		this.m_EAvatarEUIImage = UIFindHelper.FindDeepChild<ET.EUIImage>(this.uiTransform.gameObject,"EGBackGround/Top/PlayerInfo/EAvatar");
     			}
     			return this.m_EAvatarEUIImage;
     		}
     	}

		public UnityEngine.UI.Text EPlayerNameText
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EPlayerNameText == null )
     			{
		    		this.m_EPlayerNameText = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"EGBackGround/Top/PlayerInfo/EPlayerName");
     			}
     			return this.m_EPlayerNameText;
     		}
     	}

		public void DestroyWidget()
		{
			this.m_EGBackGroundRectTransform = null;
			this.m_EEnterMapButton = null;
			this.m_EEnterMapImage = null;
			this.m_EAvatarEUIButton = null;
			this.m_EAvatarEUIImage = null;
			this.m_EPlayerNameText = null;
			this.uiTransform = null;
		}

		private UnityEngine.RectTransform m_EGBackGroundRectTransform = null;
		private UnityEngine.UI.Button m_EEnterMapButton = null;
		private UnityEngine.UI.Image m_EEnterMapImage = null;
		private ET.EUIButton m_EAvatarEUIButton = null;
		private ET.EUIImage m_EAvatarEUIImage = null;
		private UnityEngine.UI.Text m_EPlayerNameText = null;
		public Transform uiTransform = null;
	}
}
