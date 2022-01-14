
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	[ObjectSystem]
	public class DlgLoginViewComponentAwakeSystem : AwakeSystem<DlgLoginViewComponent> 
	{
		public override void Awake(DlgLoginViewComponent self)
		{
			self.uiTransform = self.GetParent<UIBaseWindow>().uiTransform;
		}
	}


	[ObjectSystem]
	public class DlgLoginViewComponentDestroySystem : DestroySystem<DlgLoginViewComponent> 
	{
		public override void Destroy(DlgLoginViewComponent self)
		{
			self.m_E_LoginButton = null;
			self.m_E_LoginImage = null;
			self.m_E_AccountInputField = null;
			self.m_E_AccountImage = null;
			self.m_E_PasswordInputField = null;
			self.m_E_PasswordImage = null;
			self.uiTransform = null;
		}
	}
}
