
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	[EnableMethod]
	public  class Scroll_Item_serverTest : Entity,IAwake,IDestroy,IUIScrollItem 
	{
		private bool isCacheNode = false;
		public void SetCacheMode(bool isCache)
		{
			this.isCacheNode = isCache;
		}

		public Scroll_Item_serverTest BindTrans(Transform trans)
		{
			this.uiTransform = trans;
			return this;
		}

		public UnityEngine.UI.Image EI_serverTestImage
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
     				if( this.m_EI_serverTestImage == null )
     				{
		    			this.m_EI_serverTestImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"EI_serverTest");
     				}
     				return this.m_EI_serverTestImage;
     			}
     			else
     			{
		    		return UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"EI_serverTest");
     			}
     		}
     	}

		public UnityEngine.UI.Text E_serverTestTipText
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
     				if( this.m_E_serverTestTipText == null )
     				{
		    			this.m_E_serverTestTipText = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"E_serverTestTip");
     				}
     				return this.m_E_serverTestTipText;
     			}
     			else
     			{
		    		return UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"E_serverTestTip");
     			}
     		}
     	}

		public void DestroyWidget()
		{
			this.m_EI_serverTestImage = null;
			this.m_E_serverTestTipText = null;
			this.uiTransform = null;
		}

		private UnityEngine.UI.Image m_EI_serverTestImage = null;
		private UnityEngine.UI.Text m_E_serverTestTipText = null;
		public Transform uiTransform = null;
	}
}
