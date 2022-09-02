
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[ObjectSystem]
	public class DlgLobbyViewComponentAwakeSystem : AwakeSystem<DlgLobbyViewComponent> 
	{
		protected override void Awake(DlgLobbyViewComponent self)
		{
			self.uiTransform = self.Parent.GetParent<UIBaseWindow>().uiTransform;
		}
	}


	[ObjectSystem]
	public class DlgLobbyViewComponentDestroySystem : DestroySystem<DlgLobbyViewComponent> 
	{
		protected override void Destroy(DlgLobbyViewComponent self)
		{
			self.DestroyWidget();
		}
	}
}
