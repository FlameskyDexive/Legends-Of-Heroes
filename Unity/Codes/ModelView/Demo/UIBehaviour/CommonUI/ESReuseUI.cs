
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	[EnableMethod]
	public  class ESReuseUI : Entity,ET.IAwake<UnityEngine.Transform>,IDestroy 
	{
		public UnityEngine.UI.Image EImage_testImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EImage_testImage == null )
     			{
		    		this.m_EImage_testImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"EImage_test");
     			}
     			return this.m_EImage_testImage;
     		}
     	}

		public UnityEngine.UI.Text ELabel_testText
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_ELabel_testText == null )
     			{
		    		this.m_ELabel_testText = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"ELabel_test");
     			}
     			return this.m_ELabel_testText;
     		}
     	}

		public void DestroyWidget()
		{
			this.m_EImage_testImage = null;
			this.m_ELabel_testText = null;
			this.uiTransform = null;
		}

		private UnityEngine.UI.Image m_EImage_testImage = null;
		private UnityEngine.UI.Text m_ELabel_testText = null;
		public Transform uiTransform = null;
	}
}
