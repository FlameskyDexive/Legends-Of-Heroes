
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	public  class ESCommonUI : Entity ,ET.IAwake<UnityEngine.Transform>,IDestroy
	{
		public UnityEngine.UI.Image EImage_Test1
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EImage_Test1 == null )
     			{
		    		this.m_EImage_Test1 = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"EImage_Test1");
     			}
     			return this.m_EImage_Test1;
     		}
     	}

		public UnityEngine.UI.Text ELabel_Test2
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_ELabel_Test2 == null )
     			{
		    		this.m_ELabel_Test2 = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"ELabel_Test2");
     			}
     			return this.m_ELabel_Test2;
     		}
     	}

		public UnityEngine.UI.Image m_EImage_Test1 = null;
		public UnityEngine.UI.Text m_ELabel_Test2 = null;
		public Transform uiTransform = null;
	}
}
