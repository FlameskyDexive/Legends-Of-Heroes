
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[ComponentOf(typeof(DlgLSLogin))]
	[EnableMethod]
	public  class DlgLSLoginViewComponent : Entity,IAwake,IDestroy 
	{
		public UnityEngine.UI.InputField EAccountInputField
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EAccountInputField == null )
     			{
		    		this.m_EAccountInputField = UIFindHelper.FindDeepChild<UnityEngine.UI.InputField>(this.uiTransform.gameObject,"Panel/EAccount");
     			}
     			return this.m_EAccountInputField;
     		}
     	}

		public UnityEngine.UI.Image EAccountImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EAccountImage == null )
     			{
		    		this.m_EAccountImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"Panel/EAccount");
     			}
     			return this.m_EAccountImage;
     		}
     	}

		public UnityEngine.UI.InputField EPasswordInputField
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EPasswordInputField == null )
     			{
		    		this.m_EPasswordInputField = UIFindHelper.FindDeepChild<UnityEngine.UI.InputField>(this.uiTransform.gameObject,"Panel/EPassword");
     			}
     			return this.m_EPasswordInputField;
     		}
     	}

		public UnityEngine.UI.Image EPasswordImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EPasswordImage == null )
     			{
		    		this.m_EPasswordImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"Panel/EPassword");
     			}
     			return this.m_EPasswordImage;
     		}
     	}

		public UnityEngine.UI.Button ELoginBtnButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_ELoginBtnButton == null )
     			{
		    		this.m_ELoginBtnButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"Panel/ELoginBtn");
     			}
     			return this.m_ELoginBtnButton;
     		}
     	}

		public UnityEngine.UI.Image ELoginBtnImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_ELoginBtnImage == null )
     			{
		    		this.m_ELoginBtnImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"Panel/ELoginBtn");
     			}
     			return this.m_ELoginBtnImage;
     		}
     	}

		public void DestroyWidget()
		{
			this.m_EAccountInputField = null;
			this.m_EAccountImage = null;
			this.m_EPasswordInputField = null;
			this.m_EPasswordImage = null;
			this.m_ELoginBtnButton = null;
			this.m_ELoginBtnImage = null;
			this.uiTransform = null;
		}

		private UnityEngine.UI.InputField m_EAccountInputField = null;
		private UnityEngine.UI.Image m_EAccountImage = null;
		private UnityEngine.UI.InputField m_EPasswordInputField = null;
		private UnityEngine.UI.Image m_EPasswordImage = null;
		private UnityEngine.UI.Button m_ELoginBtnButton = null;
		private UnityEngine.UI.Image m_ELoginBtnImage = null;
		public Transform uiTransform = null;
	}
}
