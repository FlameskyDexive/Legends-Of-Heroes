
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	public  class DlgLobbyViewComponent : Entity 
	{
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
		    		this.m_EButton_EnterMap = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"Panel/EButton_EnterMap");
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
		    		this.m_EButton_EnterMapImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"Panel/EButton_EnterMap");
     			}
     			return this.m_EButton_EnterMapImage;
     		}
     	}

		public UnityEngine.UI.LoopVerticalScrollRect ELoopScrollList_Test
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
		    		this.m_ELoopScrollList_Test = UIFindHelper.FindDeepChild<UnityEngine.UI.LoopVerticalScrollRect>(this.uiTransform.gameObject,"Panel/ELoopScrollList_Test");
     			}
     			return this.m_ELoopScrollList_Test;
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
		    	   Transform subTrans = UIFindHelper.FindDeepChild<Transform>(this.uiTransform.gameObject,"ESReuseUI");
		    	   this.m_esreuseui = this.AddChild<ESReuseUI,Transform>(subTrans);
     			}
     			return this.m_esreuseui;
     		}
     	}

		public UnityEngine.UI.Image m_EButton_EnterMapImage = null;
		public UnityEngine.UI.Button m_EButton_EnterMap = null;
		public UnityEngine.UI.LoopVerticalScrollRect m_ELoopScrollList_Test = null;
		public ESReuseUI m_esreuseui = null;
		public Transform uiTransform = null;
	}
}
