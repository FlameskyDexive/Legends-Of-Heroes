
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[FriendOf(typeof(DlgLobbyViewComponent))]
	[EntitySystemOf(typeof(DlgLobbyViewComponent))]
	public static class DlgLobbyViewComponentSystem
	{
		public static void Awake(this DlgLobbyViewComponent self)
		{
			self.uiTransform = self.GetParent<UIBaseWindow>().uiTransform;
		}
		public static void Destroy(this DlgLobbyViewComponent self)
		{
			self.DestroyWidget();
		}
	}
}
