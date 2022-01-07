
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	[ObjectSystem]
	public class DlgRedDotViewComponentAwakeSystem : AwakeSystem<DlgRedDotViewComponent> 
	{
		public override void Awake(DlgRedDotViewComponent self)
		{
			self.uiTransform = self.GetParent<UIBaseWindow>().uiTransform;
		}
	}


	[ObjectSystem]
	public class DlgRedDotViewComponentDestroySystem : DestroySystem<DlgRedDotViewComponent> 
	{
		public override void Destroy(DlgRedDotViewComponent self)
		{
			self.m_EGBackGround = null;
			self.m_EButton_rootImage = null;
			self.m_EButton_root = null;
			self.m_EButton_BagImage = null;
			self.m_EButton_Bag = null;
			self.m_EButton_BagNode1Image = null;
			self.m_EButton_BagNode1 = null;
			self.m_EButton_BagNode2Image = null;
			self.m_EButton_BagNode2 = null;
			self.m_EButton_MailImage = null;
			self.m_EButton_Mail = null;
			self.m_EButton_MailNode1Image = null;
			self.m_EButton_MailNode1 = null;
			self.m_EButton_MailNode2Image = null;
			self.m_EButton_MailNode2 = null;
			self.uiTransform = null;
		}
	}
}
