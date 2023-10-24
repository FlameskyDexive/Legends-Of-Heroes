
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[ComponentOf(typeof(DlgLogin))]
	[EnableMethod]
	public  class DlgLoginViewComponent : Entity,IAwake,IDestroy 
	{
		public ESReuseUI ESReuseUI
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_esreuseui == null )
     			{
		    	   Transform subTrans = UIFindHelper.FindDeepChild<Transform>(this.uiTransform.gameObject,"Sprite_BackGround/ESReuseUI");
		    	   this.m_esreuseui = this.AddChild<ESReuseUI,Transform>(subTrans);
     			}
     			return this.m_esreuseui;
     		}
     	}

		public UnityEngine.UI.Button ELoginButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_ELoginButton == null )
     			{
		    		this.m_ELoginButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"Sprite_BackGround/ELogin");
     			}
     			return this.m_ELoginButton;
     		}
     	}

		public UnityEngine.UI.Image ELoginImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_ELoginImage == null )
     			{
		    		this.m_ELoginImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"Sprite_BackGround/ELogin");
     			}
     			return this.m_ELoginImage;
     		}
     	}

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
		    		this.m_EAccountInputField = UIFindHelper.FindDeepChild<UnityEngine.UI.InputField>(this.uiTransform.gameObject,"Sprite_BackGround/EAccount");
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
		    		this.m_EAccountImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"Sprite_BackGround/EAccount");
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
		    		this.m_EPasswordInputField = UIFindHelper.FindDeepChild<UnityEngine.UI.InputField>(this.uiTransform.gameObject,"Sprite_BackGround/EPassword");
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
		    		this.m_EPasswordImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"Sprite_BackGround/EPassword");
     			}
     			return this.m_EPasswordImage;
     		}
     	}

		public UnityEngine.UI.LoopHorizontalScrollRect ELoopTestLoopHorizontalScrollRect
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_ELoopTestLoopHorizontalScrollRect == null )
     			{
		    		this.m_ELoopTestLoopHorizontalScrollRect = UIFindHelper.FindDeepChild<UnityEngine.UI.LoopHorizontalScrollRect>(this.uiTransform.gameObject,"ELoopTest");
     			}
     			return this.m_ELoopTestLoopHorizontalScrollRect;
     		}
     	}

		public void DestroyWidget()
		{
			this.m_esreuseui = null;
			this.m_ELoginButton = null;
			this.m_ELoginImage = null;
			this.m_EAccountInputField = null;
			this.m_EAccountImage = null;
			this.m_EPasswordInputField = null;
			this.m_EPasswordImage = null;
			this.m_ELoopTestLoopHorizontalScrollRect = null;
			this.uiTransform = null;
		}

		private EntityRef<ESReuseUI> m_esreuseui = null;
		private UnityEngine.UI.Button m_ELoginButton = null;
		private UnityEngine.UI.Image m_ELoginImage = null;
		private UnityEngine.UI.InputField m_EAccountInputField = null;
		private UnityEngine.UI.Image m_EAccountImage = null;
		private UnityEngine.UI.InputField m_EPasswordInputField = null;
		private UnityEngine.UI.Image m_EPasswordImage = null;
		private UnityEngine.UI.LoopHorizontalScrollRect m_ELoopTestLoopHorizontalScrollRect = null;
		public Transform uiTransform = null;
	}
}
