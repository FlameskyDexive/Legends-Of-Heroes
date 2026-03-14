
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[ComponentOf(typeof(DlgLSLobby))]
	[EnableMethod]
	public  class DlgLSLobbyViewComponent : Entity,IAwake,IDestroy 
	{
		public UnityEngine.UI.Button EMatchButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EMatchButton == null )
     			{
		    		this.m_EMatchButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"Panel/EMatch");
     			}
     			return this.m_EMatchButton;
     		}
     	}

		public UnityEngine.UI.Image EMatchImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EMatchImage == null )
     			{
		    		this.m_EMatchImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"Panel/EMatch");
     			}
     			return this.m_EMatchImage;
     		}
     	}

		public UnityEngine.UI.InputField EReplayPathInputField
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EReplayPathInputField == null )
     			{
		    		this.m_EReplayPathInputField = UIFindHelper.FindDeepChild<UnityEngine.UI.InputField>(this.uiTransform.gameObject,"Panel/GameObject/EReplayPath");
     			}
     			return this.m_EReplayPathInputField;
     		}
     	}

		public UnityEngine.UI.Image EReplayPathImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EReplayPathImage == null )
     			{
		    		this.m_EReplayPathImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"Panel/GameObject/EReplayPath");
     			}
     			return this.m_EReplayPathImage;
     		}
     	}

		public UnityEngine.UI.Button EReplayButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EReplayButton == null )
     			{
		    		this.m_EReplayButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"Panel/GameObject/EReplay");
     			}
     			return this.m_EReplayButton;
     		}
     	}

		public UnityEngine.UI.Image EReplayImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EReplayImage == null )
     			{
		    		this.m_EReplayImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"Panel/GameObject/EReplay");
     			}
     			return this.m_EReplayImage;
     		}
     	}

		public void DestroyWidget()
		{
			this.m_EMatchButton = null;
			this.m_EMatchImage = null;
			this.m_EReplayPathInputField = null;
			this.m_EReplayPathImage = null;
			this.m_EReplayButton = null;
			this.m_EReplayImage = null;
			this.uiTransform = null;
		}

		private UnityEngine.UI.Button m_EMatchButton = null;
		private UnityEngine.UI.Image m_EMatchImage = null;
		private UnityEngine.UI.InputField m_EReplayPathInputField = null;
		private UnityEngine.UI.Image m_EReplayPathImage = null;
		private UnityEngine.UI.Button m_EReplayButton = null;
		private UnityEngine.UI.Image m_EReplayImage = null;
		public Transform uiTransform = null;
	}
}
