
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	public  class DlgLobbyViewComponent : Entity ,IDestroy,IAwake
	{
		public UnityEngine.RectTransform EGBackGround
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EGBackGround == null )
     			{
		    		this.m_EGBackGround = UIFindHelper.FindDeepChild<UnityEngine.RectTransform>(this.uiTransform.gameObject,"EGBackGround");
     			}
     			return this.m_EGBackGround;
     		}
     	}

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
		    	   Transform subTrans = UIFindHelper.FindDeepChild<Transform>(this.uiTransform.gameObject,"EGBackGround/ESReuseUI");
		    	   this.m_esreuseui = this.AddChild<ESReuseUI,Transform>(subTrans);
     			}
     			return this.m_esreuseui;
     		}
     	}

		public UnityEngine.UI.Button EButton_EnterMap
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButton_EnterMap == null )
     			{
		    		this.m_EButton_EnterMap = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"EGBackGround/EButton_EnterMap");
     			}
     			return this.m_EButton_EnterMap;
     		}
     	}

		public UnityEngine.UI.Image EButton_EnterMapImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButton_EnterMapImage == null )
     			{
		    		this.m_EButton_EnterMapImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"EGBackGround/EButton_EnterMap");
     			}
     			return this.m_EButton_EnterMapImage;
     		}
     	}

		public UnityEngine.RectTransform m_EGBackGround = null;
		public ESReuseUI m_esreuseui = null;
		public UnityEngine.UI.Image m_EButton_EnterMapImage = null;
		public UnityEngine.UI.Button m_EButton_EnterMap = null;
		public Transform uiTransform = null;
	}
}
