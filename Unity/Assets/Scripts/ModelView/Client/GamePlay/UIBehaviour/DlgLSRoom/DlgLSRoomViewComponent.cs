
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[ComponentOf(typeof(DlgLSRoom))]
	[EnableMethod]
	public  class DlgLSRoomViewComponent : Entity,IAwake,IDestroy 
	{
		public UnityEngine.UI.Text EprogressText
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EprogressText == null )
     			{
		    		this.m_EprogressText = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"Panel/Eprogress");
     			}
     			return this.m_EprogressText;
     		}
     	}

		public UnityEngine.UI.Text EpredictText
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EpredictText == null )
     			{
		    		this.m_EpredictText = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"Panel/Epredict");
     			}
     			return this.m_EpredictText;
     		}
     	}

		public UnityEngine.UI.Text EframecountText
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EframecountText == null )
     			{
		    		this.m_EframecountText = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"Panel/Eframecount");
     			}
     			return this.m_EframecountText;
     		}
     	}

		public UnityEngine.RectTransform EGReplayRectTransform
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EGReplayRectTransform == null )
     			{
		    		this.m_EGReplayRectTransform = UIFindHelper.FindDeepChild<UnityEngine.RectTransform>(this.uiTransform.gameObject,"Panel/EGReplay");
     			}
     			return this.m_EGReplayRectTransform;
     		}
     	}

		public UnityEngine.UI.InputField EjumpToCountInputField
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EjumpToCountInputField == null )
     			{
		    		this.m_EjumpToCountInputField = UIFindHelper.FindDeepChild<UnityEngine.UI.InputField>(this.uiTransform.gameObject,"Panel/EGReplay/EjumpToCount");
     			}
     			return this.m_EjumpToCountInputField;
     		}
     	}

		public UnityEngine.UI.Image EjumpToCountImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EjumpToCountImage == null )
     			{
		    		this.m_EjumpToCountImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"Panel/EGReplay/EjumpToCount");
     			}
     			return this.m_EjumpToCountImage;
     		}
     	}

		public UnityEngine.UI.Button EjumpButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EjumpButton == null )
     			{
		    		this.m_EjumpButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"Panel/EGReplay/Ejump");
     			}
     			return this.m_EjumpButton;
     		}
     	}

		public UnityEngine.UI.Image EjumpImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EjumpImage == null )
     			{
		    		this.m_EjumpImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"Panel/EGReplay/Ejump");
     			}
     			return this.m_EjumpImage;
     		}
     	}

		public UnityEngine.UI.Button EspeedButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EspeedButton == null )
     			{
		    		this.m_EspeedButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"Panel/EGReplay/Espeed");
     			}
     			return this.m_EspeedButton;
     		}
     	}

		public UnityEngine.UI.Image EspeedImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EspeedImage == null )
     			{
		    		this.m_EspeedImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"Panel/EGReplay/Espeed");
     			}
     			return this.m_EspeedImage;
     		}
     	}

		public UnityEngine.RectTransform EGPlayRectTransform
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EGPlayRectTransform == null )
     			{
		    		this.m_EGPlayRectTransform = UIFindHelper.FindDeepChild<UnityEngine.RectTransform>(this.uiTransform.gameObject,"Panel/EGPlay");
     			}
     			return this.m_EGPlayRectTransform;
     		}
     	}

		public UnityEngine.UI.InputField E_SaveNameInputField
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_SaveNameInputField == null )
     			{
		    		this.m_E_SaveNameInputField = UIFindHelper.FindDeepChild<UnityEngine.UI.InputField>(this.uiTransform.gameObject,"Panel/EGPlay/E_SaveName");
     			}
     			return this.m_E_SaveNameInputField;
     		}
     	}

		public UnityEngine.UI.Image E_SaveNameImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_SaveNameImage == null )
     			{
		    		this.m_E_SaveNameImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"Panel/EGPlay/E_SaveName");
     			}
     			return this.m_E_SaveNameImage;
     		}
     	}

		public UnityEngine.UI.Button E_SaveReplayButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_SaveReplayButton == null )
     			{
		    		this.m_E_SaveReplayButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"Panel/EGPlay/E_SaveReplay");
     			}
     			return this.m_E_SaveReplayButton;
     		}
     	}

		public UnityEngine.UI.Image E_SaveReplayImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_SaveReplayImage == null )
     			{
		    		this.m_E_SaveReplayImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"Panel/EGPlay/E_SaveReplay");
     			}
     			return this.m_E_SaveReplayImage;
     		}
     	}

		public void DestroyWidget()
		{
			this.m_EprogressText = null;
			this.m_EpredictText = null;
			this.m_EframecountText = null;
			this.m_EGReplayRectTransform = null;
			this.m_EjumpToCountInputField = null;
			this.m_EjumpToCountImage = null;
			this.m_EjumpButton = null;
			this.m_EjumpImage = null;
			this.m_EspeedButton = null;
			this.m_EspeedImage = null;
			this.m_EGPlayRectTransform = null;
			this.m_E_SaveNameInputField = null;
			this.m_E_SaveNameImage = null;
			this.m_E_SaveReplayButton = null;
			this.m_E_SaveReplayImage = null;
			this.uiTransform = null;
		}

		private UnityEngine.UI.Text m_EprogressText = null;
		private UnityEngine.UI.Text m_EpredictText = null;
		private UnityEngine.UI.Text m_EframecountText = null;
		private UnityEngine.RectTransform m_EGReplayRectTransform = null;
		private UnityEngine.UI.InputField m_EjumpToCountInputField = null;
		private UnityEngine.UI.Image m_EjumpToCountImage = null;
		private UnityEngine.UI.Button m_EjumpButton = null;
		private UnityEngine.UI.Image m_EjumpImage = null;
		private UnityEngine.UI.Button m_EspeedButton = null;
		private UnityEngine.UI.Image m_EspeedImage = null;
		private UnityEngine.RectTransform m_EGPlayRectTransform = null;
		private UnityEngine.UI.InputField m_E_SaveNameInputField = null;
		private UnityEngine.UI.Image m_E_SaveNameImage = null;
		private UnityEngine.UI.Button m_E_SaveReplayButton = null;
		private UnityEngine.UI.Image m_E_SaveReplayImage = null;
		public Transform uiTransform = null;
	}
}
