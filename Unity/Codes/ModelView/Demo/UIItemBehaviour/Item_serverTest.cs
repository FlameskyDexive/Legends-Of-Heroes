
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	public  class Scroll_Item_serverTest : Entity ,IAwake,IDestroy
	{
		private bool isCacheNode = true;
		public void SetCacheMode(bool isCache)
		{
			this.isCacheNode = isCache;
		}

		public Scroll_Item_serverTest BindTrans(Transform trans)
		{
			this.uiTransform = trans;
			return this;
		}

		public UnityEngine.UI.Image EImage_serverTest
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if (this.isCacheNode)
     			{
     				if( this.m_EImage_serverTest == null )
     				{
		    			this.m_EImage_serverTest = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"EImage_serverTest");
     				}
     				return this.m_EImage_serverTest;
     			}
     			else
     			{
		    		return UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"EImage_serverTest");
     			}
     		}
     	}

		public UnityEngine.UI.Text ELabel_serverTest
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if (this.isCacheNode)
     			{
     				if( this.m_ELabel_serverTest == null )
     				{
		    			this.m_ELabel_serverTest = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"ELabel_serverTest");
     				}
     				return this.m_ELabel_serverTest;
     			}
     			else
     			{
		    		return UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"ELabel_serverTest");
     			}
     		}
     	}

		public UnityEngine.UI.Image m_EImage_serverTest = null;
		public UnityEngine.UI.Text m_ELabel_serverTest = null;
		public Transform uiTransform = null;
	}
}
