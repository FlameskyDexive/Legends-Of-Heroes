
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	public  class DlgHelperViewComponent : Entity,IAwake,IDestroy 
	{
		public UnityEngine.UI.Text ELabel_Tip
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_ELabel_Tip == null )
     			{
		    		this.m_ELabel_Tip = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"ELabel_Tip");
     			}
     			return this.m_ELabel_Tip;
     		}
     	}

		public UnityEngine.UI.Text m_ELabel_Tip = null;
		public Transform uiTransform = null;
	}
}
