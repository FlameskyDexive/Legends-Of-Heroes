
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	public  class DlgTestViewComponent : Entity,IAwake,IDestroy 
	{
		public UnityEngine.UI.Button EButton_Test
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButton_Test == null )
     			{
		    		this.m_EButton_Test = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"EButton_Test");
     			}
     			return this.m_EButton_Test;
     		}
     	}

		public UnityEngine.UI.Image EButton_TestImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButton_TestImage == null )
     			{
		    		this.m_EButton_TestImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"EButton_Test");
     			}
     			return this.m_EButton_TestImage;
     		}
     	}

		public UnityEngine.UI.Text ELabel_Test
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_ELabel_Test == null )
     			{
		    		this.m_ELabel_Test = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"EButton_Test/ELabel_Test");
     			}
     			return this.m_ELabel_Test;
     		}
     	}

		public UnityEngine.UI.Image m_EButton_TestImage = null;
		public UnityEngine.UI.Button m_EButton_Test = null;
		public UnityEngine.UI.Text m_ELabel_Test = null;
		public Transform uiTransform = null;
	}
}
