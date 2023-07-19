
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[FriendOf(typeof(DlgRoomViewComponent))]
	[EntitySystemOf(typeof(DlgRoomViewComponent))]
	public static class DlgRoomViewComponentSystem
	{
		public static void Awake(this DlgRoomViewComponent self)
		{
			self.uiTransform = self.GetParent<UIBaseWindow>().uiTransform;
		}
		public static void Destroy(this DlgRoomViewComponent self)
		{
			self.DestroyWidget();
		}
	}
}
