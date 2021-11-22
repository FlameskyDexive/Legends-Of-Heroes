
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	public  class ESReuseUI : Entity 
	{
		public UnityEngine.UI.Image EImage_test
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EImage_test == null )
     			{
		    		this.m_EImage_test = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"EImage_test");
     			}
     			return this.m_EImage_test;
     		}
     	}

		public UnityEngine.UI.Text ELabel_test
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_ELabel_test == null )
     			{
		    		this.m_ELabel_test = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"ELabel_test");
     			}
     			return this.m_ELabel_test;
     		}
     	}

		public UnityEngine.UI.Image m_EImage_test = null;
		public UnityEngine.UI.Text m_ELabel_test = null;
		public Transform uiTransform = null;
	}
}
