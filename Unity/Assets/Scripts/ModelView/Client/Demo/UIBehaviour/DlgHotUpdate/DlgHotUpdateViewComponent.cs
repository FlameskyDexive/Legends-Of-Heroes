
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[ComponentOf(typeof(UIBaseWindow))]
	[EnableMethod]
	public  class DlgHotUpdateViewComponent : Entity,IAwake,IDestroy 
	{
		public UnityEngine.UI.Text E_TxtLoadingText
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_TxtLoadingText == null )
     			{
		    		this.m_E_TxtLoadingText = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"E_TxtLoading");
     			}
     			return this.m_E_TxtLoadingText;
     		}
     	}

		public UnityEngine.UI.Image E_ImgLoadingImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_ImgLoadingImage == null )
     			{
		    		this.m_E_ImgLoadingImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"LoadingBg/E_ImgLoading");
     			}
     			return this.m_E_ImgLoadingImage;
     		}
     	}

		public void DestroyWidget()
		{
			this.m_E_TxtLoadingText = null;
			this.m_E_ImgLoadingImage = null;
			this.uiTransform = null;
		}

		private UnityEngine.UI.Text m_E_TxtLoadingText = null;
		private UnityEngine.UI.Image m_E_ImgLoadingImage = null;
		public Transform uiTransform = null;
	}
}
