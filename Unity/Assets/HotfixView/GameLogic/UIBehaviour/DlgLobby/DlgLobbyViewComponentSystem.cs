
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
			self.m_EButton_EnterMapImage = null;
			self.m_EButton_EnterMap = null;
			self.m_ELoopScrollList_Test = null;
			self.m_esreuseui?.Dispose();
			self.m_esreuseui = null;
			self.uiTransform = null;
		}
	}
}
