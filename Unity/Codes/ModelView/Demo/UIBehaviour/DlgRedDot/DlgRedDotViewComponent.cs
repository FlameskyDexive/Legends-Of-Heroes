
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	[ComponentOf(typeof(UIBaseWindow))]
	[EnableMethod]
	public  class DlgRedDotViewComponent : Entity,IAwake,IDestroy 
	{
		public UnityEngine.RectTransform EGBackGroundRectTransform
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EGBackGroundRectTransform == null )
     			{
		    		this.m_EGBackGroundRectTransform = UIFindHelper.FindDeepChild<UnityEngine.RectTransform>(this.uiTransform.gameObject,"EGBackGround");
     			}
     			return this.m_EGBackGroundRectTransform;
     		}
     	}

		public UnityEngine.UI.Button EButton_rootButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButton_rootButton == null )
     			{
		    		this.m_EButton_rootButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"EButton_root");
     			}
     			return this.m_EButton_rootButton;
     		}
     	}

		public UnityEngine.UI.Image EButton_rootImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButton_rootImage == null )
     			{
		    		this.m_EButton_rootImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"EButton_root");
     			}
     			return this.m_EButton_rootImage;
     		}
     	}

		public UnityEngine.UI.Button EButton_BagButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButton_BagButton == null )
     			{
		    		this.m_EButton_BagButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"EButton_Bag");
     			}
     			return this.m_EButton_BagButton;
     		}
     	}

		public UnityEngine.UI.Image EButton_BagImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButton_BagImage == null )
     			{
		    		this.m_EButton_BagImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"EButton_Bag");
     			}
     			return this.m_EButton_BagImage;
     		}
     	}

		public UnityEngine.UI.Button EButton_BagNode1Button
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButton_BagNode1Button == null )
     			{
		    		this.m_EButton_BagNode1Button = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"EButton_BagNode1");
     			}
     			return this.m_EButton_BagNode1Button;
     		}
     	}

		public UnityEngine.UI.Image EButton_BagNode1Image
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButton_BagNode1Image == null )
     			{
		    		this.m_EButton_BagNode1Image = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"EButton_BagNode1");
     			}
     			return this.m_EButton_BagNode1Image;
     		}
     	}

		public UnityEngine.UI.Button EButton_BagNode2Button
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButton_BagNode2Button == null )
     			{
		    		this.m_EButton_BagNode2Button = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"EButton_BagNode2");
     			}
     			return this.m_EButton_BagNode2Button;
     		}
     	}

		public UnityEngine.UI.Image EButton_BagNode2Image
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButton_BagNode2Image == null )
     			{
		    		this.m_EButton_BagNode2Image = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"EButton_BagNode2");
     			}
     			return this.m_EButton_BagNode2Image;
     		}
     	}

		public UnityEngine.UI.Button EButton_MailButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButton_MailButton == null )
     			{
		    		this.m_EButton_MailButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"EButton_Mail");
     			}
     			return this.m_EButton_MailButton;
     		}
     	}

		public UnityEngine.UI.Image EButton_MailImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButton_MailImage == null )
     			{
		    		this.m_EButton_MailImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"EButton_Mail");
     			}
     			return this.m_EButton_MailImage;
     		}
     	}

		public UnityEngine.UI.Button EButton_MailNode1Button
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButton_MailNode1Button == null )
     			{
		    		this.m_EButton_MailNode1Button = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"EButton_MailNode1");
     			}
     			return this.m_EButton_MailNode1Button;
     		}
     	}

		public UnityEngine.UI.Image EButton_MailNode1Image
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButton_MailNode1Image == null )
     			{
		    		this.m_EButton_MailNode1Image = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"EButton_MailNode1");
     			}
     			return this.m_EButton_MailNode1Image;
     		}
     	}

		public UnityEngine.UI.Button EButton_MailNode2Button
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButton_MailNode2Button == null )
     			{
		    		this.m_EButton_MailNode2Button = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"EButton_MailNode2");
     			}
     			return this.m_EButton_MailNode2Button;
     		}
     	}

		public UnityEngine.UI.Image EButton_MailNode2Image
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButton_MailNode2Image == null )
     			{
		    		this.m_EButton_MailNode2Image = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"EButton_MailNode2");
     			}
     			return this.m_EButton_MailNode2Image;
     		}
     	}

		public void DestroyWidget()
		{
			this.m_EGBackGroundRectTransform = null;
			this.m_EButton_rootButton = null;
			this.m_EButton_rootImage = null;
			this.m_EButton_BagButton = null;
			this.m_EButton_BagImage = null;
			this.m_EButton_BagNode1Button = null;
			this.m_EButton_BagNode1Image = null;
			this.m_EButton_BagNode2Button = null;
			this.m_EButton_BagNode2Image = null;
			this.m_EButton_MailButton = null;
			this.m_EButton_MailImage = null;
			this.m_EButton_MailNode1Button = null;
			this.m_EButton_MailNode1Image = null;
			this.m_EButton_MailNode2Button = null;
			this.m_EButton_MailNode2Image = null;
			this.uiTransform = null;
		}

		private UnityEngine.RectTransform m_EGBackGroundRectTransform = null;
		private UnityEngine.UI.Button m_EButton_rootButton = null;
		private UnityEngine.UI.Image m_EButton_rootImage = null;
		private UnityEngine.UI.Button m_EButton_BagButton = null;
		private UnityEngine.UI.Image m_EButton_BagImage = null;
		private UnityEngine.UI.Button m_EButton_BagNode1Button = null;
		private UnityEngine.UI.Image m_EButton_BagNode1Image = null;
		private UnityEngine.UI.Button m_EButton_BagNode2Button = null;
		private UnityEngine.UI.Image m_EButton_BagNode2Image = null;
		private UnityEngine.UI.Button m_EButton_MailButton = null;
		private UnityEngine.UI.Image m_EButton_MailImage = null;
		private UnityEngine.UI.Button m_EButton_MailNode1Button = null;
		private UnityEngine.UI.Image m_EButton_MailNode1Image = null;
		private UnityEngine.UI.Button m_EButton_MailNode2Button = null;
		private UnityEngine.UI.Image m_EButton_MailNode2Image = null;
		public Transform uiTransform = null;
	}
}
