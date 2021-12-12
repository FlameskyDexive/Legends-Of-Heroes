
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	public  class DlgRedDotViewComponent : Entity 
	{
		public UnityEngine.RectTransform EGBackGround
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EGBackGround == null )
     			{
		    		this.m_EGBackGround = UIFindHelper.FindDeepChild<UnityEngine.RectTransform>(this.uiTransform.gameObject,"EGBackGround");
     			}
     			return this.m_EGBackGround;
     		}
     	}

		public UnityEngine.UI.Button EButton_root
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButton_root == null )
     			{
		    		this.m_EButton_root = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"EButton_root");
     			}
     			return this.m_EButton_root;
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

		public UnityEngine.UI.Button EButton_Bag
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButton_Bag == null )
     			{
		    		this.m_EButton_Bag = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"EButton_Bag");
     			}
     			return this.m_EButton_Bag;
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

		public UnityEngine.UI.Button EButton_BagNode1
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButton_BagNode1 == null )
     			{
		    		this.m_EButton_BagNode1 = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"EButton_BagNode1");
     			}
     			return this.m_EButton_BagNode1;
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

		public UnityEngine.UI.Button EButton_BagNode2
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButton_BagNode2 == null )
     			{
		    		this.m_EButton_BagNode2 = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"EButton_BagNode2");
     			}
     			return this.m_EButton_BagNode2;
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

		public UnityEngine.UI.Button EButton_Mail
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButton_Mail == null )
     			{
		    		this.m_EButton_Mail = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"EButton_Mail");
     			}
     			return this.m_EButton_Mail;
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

		public UnityEngine.UI.Button EButton_MailNode1
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButton_MailNode1 == null )
     			{
		    		this.m_EButton_MailNode1 = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"EButton_MailNode1");
     			}
     			return this.m_EButton_MailNode1;
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

		public UnityEngine.UI.Button EButton_MailNode2
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButton_MailNode2 == null )
     			{
		    		this.m_EButton_MailNode2 = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"EButton_MailNode2");
     			}
     			return this.m_EButton_MailNode2;
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

		public UnityEngine.RectTransform m_EGBackGround = null;
		public UnityEngine.UI.Image m_EButton_rootImage = null;
		public UnityEngine.UI.Button m_EButton_root = null;
		public UnityEngine.UI.Image m_EButton_BagImage = null;
		public UnityEngine.UI.Button m_EButton_Bag = null;
		public UnityEngine.UI.Image m_EButton_BagNode1Image = null;
		public UnityEngine.UI.Button m_EButton_BagNode1 = null;
		public UnityEngine.UI.Image m_EButton_BagNode2Image = null;
		public UnityEngine.UI.Button m_EButton_BagNode2 = null;
		public UnityEngine.UI.Image m_EButton_MailImage = null;
		public UnityEngine.UI.Button m_EButton_Mail = null;
		public UnityEngine.UI.Image m_EButton_MailNode1Image = null;
		public UnityEngine.UI.Button m_EButton_MailNode1 = null;
		public UnityEngine.UI.Image m_EButton_MailNode2Image = null;
		public UnityEngine.UI.Button m_EButton_MailNode2 = null;
		public Transform uiTransform = null;
	}
}
