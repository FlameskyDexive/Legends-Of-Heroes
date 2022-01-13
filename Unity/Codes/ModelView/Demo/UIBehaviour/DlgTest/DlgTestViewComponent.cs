
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	public  class DlgTestViewComponent : Entity,IAwake,IDestroy 
	{
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
		    		this.m_ELabel_Test = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"ELabel_Test");
     			}
     			return this.m_ELabel_Test;
     		}
     	}

		public UnityEngine.UI.Text m_ELabel_Test = null;
		public Transform uiTransform = null;
	}
}
