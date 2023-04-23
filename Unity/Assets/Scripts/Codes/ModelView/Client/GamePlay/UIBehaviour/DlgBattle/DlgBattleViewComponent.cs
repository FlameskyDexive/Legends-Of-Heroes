
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[ComponentOf(typeof(UIBaseWindow))]
	[EnableMethod]
	public  class DlgBattleViewComponent : Entity,IAwake,IDestroy 
	{
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
		    		this.m_E_PlayerNameText = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"Top/PlayerRoot/E_PlayerName");
     			}
     			return this.m_E_PlayerNameText;
     		}
     	}

		public UnityEngine.UI.Joystick E_JoystickJoystick
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_JoystickJoystick == null )
     			{
		    		this.m_E_JoystickJoystick = UIFindHelper.FindDeepChild<UnityEngine.UI.Joystick>(this.uiTransform.gameObject,"Bottom/Joy/E_Joystick");
     			}
     			return this.m_E_JoystickJoystick;
     		}
     	}

		public UnityEngine.UI.Image E_JoystickImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_JoystickImage == null )
     			{
		    		this.m_E_JoystickImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"Bottom/Joy/E_Joystick");
     			}
     			return this.m_E_JoystickImage;
     		}
     	}

		public UnityEngine.UI.Button EBtnSkill1Button
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EBtnSkill1Button == null )
     			{
		    		this.m_EBtnSkill1Button = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"Bottom/Skill/EBtnSkill1");
     			}
     			return this.m_EBtnSkill1Button;
     		}
     	}

		public UnityEngine.UI.Text ETextSkill1Text
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_ETextSkill1Text == null )
     			{
		    		this.m_ETextSkill1Text = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"Bottom/Skill/EBtnSkill1/ETextSkill1");
     			}
     			return this.m_ETextSkill1Text;
     		}
     	}

		public UnityEngine.UI.Image EIconSkill1Image
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EIconSkill1Image == null )
     			{
		    		this.m_EIconSkill1Image = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"Bottom/Skill/EBtnSkill1/EIconSkill1");
     			}
     			return this.m_EIconSkill1Image;
     		}
     	}

		public UnityEngine.UI.Image EIMgCD1Image
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EIMgCD1Image == null )
     			{
		    		this.m_EIMgCD1Image = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"Bottom/Skill/EBtnSkill1/EIMgCD1");
     			}
     			return this.m_EIMgCD1Image;
     		}
     	}

		public UnityEngine.UI.Image EImgMask1Image
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EImgMask1Image == null )
     			{
		    		this.m_EImgMask1Image = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"Bottom/Skill/EBtnSkill1/EImgMask1");
     			}
     			return this.m_EImgMask1Image;
     		}
     	}

		public UnityEngine.UI.Button EBtnSkill2Button
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EBtnSkill2Button == null )
     			{
		    		this.m_EBtnSkill2Button = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"Bottom/Skill/EBtnSkill2");
     			}
     			return this.m_EBtnSkill2Button;
     		}
     	}

		public UnityEngine.UI.Text ETextSkill2Text
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_ETextSkill2Text == null )
     			{
		    		this.m_ETextSkill2Text = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"Bottom/Skill/EBtnSkill2/ETextSkill2");
     			}
     			return this.m_ETextSkill2Text;
     		}
     	}

		public UnityEngine.UI.Image EIconSkill2Image
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EIconSkill2Image == null )
     			{
		    		this.m_EIconSkill2Image = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"Bottom/Skill/EBtnSkill2/EIconSkill2");
     			}
     			return this.m_EIconSkill2Image;
     		}
     	}

		public UnityEngine.UI.Image EIMgCD2Image
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EIMgCD2Image == null )
     			{
		    		this.m_EIMgCD2Image = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"Bottom/Skill/EBtnSkill2/EIMgCD2");
     			}
     			return this.m_EIMgCD2Image;
     		}
     	}

		public UnityEngine.UI.Image EImgMask2Image
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EImgMask2Image == null )
     			{
		    		this.m_EImgMask2Image = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"Bottom/Skill/EBtnSkill2/EImgMask2");
     			}
     			return this.m_EImgMask2Image;
     		}
     	}

		public void DestroyWidget()
		{
			this.m_E_PlayerNameText = null;
			this.m_E_JoystickJoystick = null;
			this.m_E_JoystickImage = null;
			this.m_EBtnSkill1Button = null;
			this.m_ETextSkill1Text = null;
			this.m_EIconSkill1Image = null;
			this.m_EIMgCD1Image = null;
			this.m_EImgMask1Image = null;
			this.m_EBtnSkill2Button = null;
			this.m_ETextSkill2Text = null;
			this.m_EIconSkill2Image = null;
			this.m_EIMgCD2Image = null;
			this.m_EImgMask2Image = null;
			this.uiTransform = null;
		}

		private UnityEngine.UI.Text m_E_PlayerNameText = null;
		private UnityEngine.UI.Joystick m_E_JoystickJoystick = null;
		private UnityEngine.UI.Image m_E_JoystickImage = null;
		private UnityEngine.UI.Button m_EBtnSkill1Button = null;
		private UnityEngine.UI.Text m_ETextSkill1Text = null;
		private UnityEngine.UI.Image m_EIconSkill1Image = null;
		private UnityEngine.UI.Image m_EIMgCD1Image = null;
		private UnityEngine.UI.Image m_EImgMask1Image = null;
		private UnityEngine.UI.Button m_EBtnSkill2Button = null;
		private UnityEngine.UI.Text m_ETextSkill2Text = null;
		private UnityEngine.UI.Image m_EIconSkill2Image = null;
		private UnityEngine.UI.Image m_EIMgCD2Image = null;
		private UnityEngine.UI.Image m_EImgMask2Image = null;
		public Transform uiTransform = null;
	}
}
