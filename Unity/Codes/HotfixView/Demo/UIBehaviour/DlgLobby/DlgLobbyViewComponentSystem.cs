
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	[ObjectSystem]
	public class DlgLobbyViewComponentAwakeSystem : AwakeSystem<DlgLobbyViewComponent> 
	{
		public override void Awake(DlgLobbyViewComponent self)
		{
			self.uiTransform = self.GetParent<UIBaseWindow>().uiTransform;
		}
	}


	[ObjectSystem]
	public class DlgLobbyViewComponentDestroySystem : DestroySystem<DlgLobbyViewComponent> 
	{
		public override void Destroy(DlgLobbyViewComponent self)
		{
			self.m_EGBackGroundRectTransform = null;
			self.m_E_EnterMapButton = null;
			self.m_E_EnterMapImage = null;
			self.uiTransform = null;
		}
	}
}
