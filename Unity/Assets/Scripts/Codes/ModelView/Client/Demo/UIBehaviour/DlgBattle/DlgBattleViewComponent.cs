
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

		public void DestroyWidget()
		{
			this.m_E_PlayerNameText = null;
			this.m_E_JoystickJoystick = null;
			this.m_E_JoystickImage = null;
			this.uiTransform = null;
		}

		private UnityEngine.UI.Text m_E_PlayerNameText = null;
		private UnityEngine.UI.Joystick m_E_JoystickJoystick = null;
		private UnityEngine.UI.Image m_E_JoystickImage = null;
		public Transform uiTransform = null;
	}
}
