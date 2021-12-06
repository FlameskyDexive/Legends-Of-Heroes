
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	[ObjectSystem]
	public class DlgTestViewComponentAwakeSystem : AwakeSystem<DlgTestViewComponent> 
	{
		public override void Awake(DlgTestViewComponent self)
		{
			self.uiTransform = self.GetParent<UIBaseWindow>().uiTransform;
		}
	}


	[ObjectSystem]
	public class DlgTestViewComponentDestroySystem : DestroySystem<DlgTestViewComponent> 
	{
		public override void Destroy(DlgTestViewComponent self)
		{
			self.m_EGBackGround = null;
			self.m_esreuseui?.Dispose();
			self.m_esreuseui = null;
			self.m_EButton_EnterMapImage = null;
			self.m_EButton_EnterMap = null;
			self.m_EButton_TestImage = null;
			self.m_EButton_Test = null;
			self.m_EText_Test = null;
			self.m_escommonui?.Dispose();
			self.m_escommonui = null;
			self.uiTransform = null;
		}
	}
}
