
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	public partial class Scroll_Item_test : Entity 
	{
		private Transform uiTransform;
		private bool isCacheNode = true;
		public void SetCacheMode(bool isCache)
		{
			this.isCacheNode = isCache;
		}

		public Scroll_Item_test BindTrans(Transform trans)
		{
			this.uiTransform = trans;
			return this;
		}

		public UnityEngine.UI.Text ELabel_Content
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
     				if( this.m_ELabel_Content == null )
     				{
		    			this.m_ELabel_Content = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"ELabel_Content");
     				}
     				return this.m_ELabel_Content;
     			}
     			else
     			{
		    		return UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"ELabel_Content");
     			}
     		}
     	}

		public override void Dispose()
		{
			this.m_ELabel_Content = null;
			this.uiTransform = null;
			base.Dispose();
		}

		public UnityEngine.UI.Text m_ELabel_Content = null;
	}
}
