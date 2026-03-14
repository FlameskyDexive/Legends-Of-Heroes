
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[EntitySystemOf(typeof(DlgLSLobbyViewComponent))]
	[FriendOfAttribute(typeof(ET.Client.DlgLSLobbyViewComponent))]
	public static partial class DlgLSLobbyViewComponentSystem
	{
		[EntitySystem]
		private static void Awake(this DlgLSLobbyViewComponent self)
		{
			self.uiTransform = self.Parent.GetParent<UIBaseWindow>().uiTransform;
		}


		[EntitySystem]
		private static void Destroy(this DlgLSLobbyViewComponent self)
		{
			self.DestroyWidget();
		}
	}


}
