
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[ComponentOf(typeof(UIBaseWindow))]
	[EnableMethod]
	public  class DlgRoomViewComponent : Entity,IAwake,IDestroy 
	{
		public UnityEngine.UI.Button E_ConfirmButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_ConfirmButton == null )
     			{
		    		this.m_E_ConfirmButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"BackGround/E_Confirm");
     			}
     			return this.m_E_ConfirmButton;
     		}
     	}

		public UnityEngine.UI.Image E_ConfirmImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_ConfirmImage == null )
     			{
		    		this.m_E_ConfirmImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"BackGround/E_Confirm");
     			}
     			return this.m_E_ConfirmImage;
     		}
     	}

		public UnityEngine.UI.LoopHorizontalScrollRect ELoopScrollList_RolesLoopHorizontalScrollRect
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_ELoopScrollList_RolesLoopHorizontalScrollRect == null )
     			{
		    		this.m_ELoopScrollList_RolesLoopHorizontalScrollRect = UIFindHelper.FindDeepChild<UnityEngine.UI.LoopHorizontalScrollRect>(this.uiTransform.gameObject,"BackGround/ELoopScrollList_Roles");
     			}
     			return this.m_ELoopScrollList_RolesLoopHorizontalScrollRect;
     		}
     	}

		public UnityEngine.UI.Button E_CancelButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_CancelButton == null )
     			{
		    		this.m_E_CancelButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"BackGround/E_Cancel");
     			}
     			return this.m_E_CancelButton;
     		}
     	}

		public UnityEngine.UI.Image E_CancelImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_CancelImage == null )
     			{
		    		this.m_E_CancelImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"BackGround/E_Cancel");
     			}
     			return this.m_E_CancelImage;
     		}
     	}

		public void DestroyWidget()
		{
			this.m_E_ConfirmButton = null;
			this.m_E_ConfirmImage = null;
			this.m_ELoopScrollList_RolesLoopHorizontalScrollRect = null;
			this.m_E_CancelButton = null;
			this.m_E_CancelImage = null;
			this.uiTransform = null;
		}

		private UnityEngine.UI.Button m_E_ConfirmButton = null;
		private UnityEngine.UI.Image m_E_ConfirmImage = null;
		private UnityEngine.UI.LoopHorizontalScrollRect m_ELoopScrollList_RolesLoopHorizontalScrollRect = null;
		private UnityEngine.UI.Button m_E_CancelButton = null;
		private UnityEngine.UI.Image m_E_CancelImage = null;
		public Transform uiTransform = null;
	}
}
