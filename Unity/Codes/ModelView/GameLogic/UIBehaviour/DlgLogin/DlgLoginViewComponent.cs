
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	public  class DlgLoginViewComponent : Entity 
	{
		public UnityEngine.UI.Button EButton_LoginBtn
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButton_LoginBtn == null )
     			{
		    		this.m_EButton_LoginBtn = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"Sprite_BackGround/EButton_LoginBtn");
     			}
     			return this.m_EButton_LoginBtn;
     		}
     	}

		public UnityEngine.UI.Image EButton_LoginBtnImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButton_LoginBtnImage == null )
     			{
		    		this.m_EButton_LoginBtnImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"Sprite_BackGround/EButton_LoginBtn");
     			}
     			return this.m_EButton_LoginBtnImage;
     		}
     	}

		public UnityEngine.UI.InputField EInput_Account
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EInput_Account == null )
     			{
		    		this.m_EInput_Account = UIFindHelper.FindDeepChild<UnityEngine.UI.InputField>(this.uiTransform.gameObject,"Sprite_BackGround/EInput_Account");
     			}
     			return this.m_EInput_Account;
     		}
     	}

		public UnityEngine.UI.InputField EInput_Password
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EInput_Password == null )
     			{
		    		this.m_EInput_Password = UIFindHelper.FindDeepChild<UnityEngine.UI.InputField>(this.uiTransform.gameObject,"Sprite_BackGround/EInput_Password");
     			}
     			return this.m_EInput_Password;
     		}
     	}

		public UnityEngine.UI.LoopHorizontalScrollRect ELoopScrollList_Test
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_ELoopScrollList_Test == null )
     			{
		    		this.m_ELoopScrollList_Test = UIFindHelper.FindDeepChild<UnityEngine.UI.LoopHorizontalScrollRect>(this.uiTransform.gameObject,"Sprite_BackGround/ELoopScrollList_Test");
     			}
     			return this.m_ELoopScrollList_Test;
     		}
     	}

		public UnityEngine.UI.Image m_EButton_LoginBtnImage = null;
		public UnityEngine.UI.Button m_EButton_LoginBtn = null;
		public UnityEngine.UI.InputField m_EInput_Account = null;
		public UnityEngine.UI.InputField m_EInput_Password = null;
		public UnityEngine.UI.LoopHorizontalScrollRect m_ELoopScrollList_Test = null;
		public Transform uiTransform = null;
	}
}
