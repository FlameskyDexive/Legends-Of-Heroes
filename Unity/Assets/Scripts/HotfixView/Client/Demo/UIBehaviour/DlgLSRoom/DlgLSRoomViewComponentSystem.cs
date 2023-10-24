
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[EntitySystemOf(typeof(DlgLSRoomViewComponent))]
	[FriendOfAttribute(typeof(ET.Client.DlgLSRoomViewComponent))]
	public static partial class DlgLSRoomViewComponentSystem
	{
		[EntitySystem]
		private static void Awake(this DlgLSRoomViewComponent self)
		{
			self.uiTransform = self.Parent.GetParent<UIBaseWindow>().uiTransform;
		}


		[EntitySystem]
		private static void Destroy(this DlgLSRoomViewComponent self)
		{
			self.DestroyWidget();
		}
	}


}
