
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[ComponentOf(typeof(UIBaseWindow))]
	[EnableMethod]
	public  class DlgHotUpdateViewComponent : Entity,IAwake,IDestroy 
	{
		public UnityEngine.UI.Text E_TxtLoaidngText
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_TxtLoaidngText == null )
     			{
		    		this.m_E_TxtLoaidngText = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"E_TxtLoaidng");
     			}
     			return this.m_E_TxtLoaidngText;
     		}
     	}

		public UnityEngine.UI.Image E_ImgLoaidngImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_ImgLoaidngImage == null )
     			{
		    		this.m_E_ImgLoaidngImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"LoadingBg/E_ImgLoaidng");
     			}
     			return this.m_E_ImgLoaidngImage;
     		}
     	}

		public void DestroyWidget()
		{
			this.m_E_TxtLoaidngText = null;
			this.m_E_ImgLoaidngImage = null;
			this.uiTransform = null;
		}

		private UnityEngine.UI.Text m_E_TxtLoaidngText = null;
		private UnityEngine.UI.Image m_E_ImgLoaidngImage = null;
		public Transform uiTransform = null;
	}
}
