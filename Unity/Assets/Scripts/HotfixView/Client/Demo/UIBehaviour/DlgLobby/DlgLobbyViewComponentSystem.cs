
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[FriendOf(typeof(DlgLobbyViewComponent))]
	[EntitySystemOf(typeof(DlgLobbyViewComponent))]
	public static partial class DlgLobbyViewComponentSystem
	{
		[EntitySystem]
		public static void Awake(this DlgLobbyViewComponent self)
		{
			self.uiTransform = self.GetParent<UIBaseWindow>().uiTransform;
		}
		[EntitySystem]
		public static void Destroy(this DlgLobbyViewComponent self)
		{
			self.DestroyWidget();
		}
	}
}
