
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[EntitySystemOf(typeof(DlgRoomViewComponent))]
	[FriendOfAttribute(typeof(ET.Client.DlgRoomViewComponent))]
	public static partial class DlgRoomViewComponentSystem
	{
		[EntitySystem]
		private static void Awake(this DlgRoomViewComponent self)
		{
			self.uiTransform = self.Parent.GetParent<UIBaseWindow>().uiTransform;
		}


		[EntitySystem]
		private static void Destroy(this DlgRoomViewComponent self)
		{
			self.DestroyWidget();
		}
	}


}
