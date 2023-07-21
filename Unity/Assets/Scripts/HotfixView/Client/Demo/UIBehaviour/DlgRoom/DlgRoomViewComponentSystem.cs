
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[FriendOf(typeof(DlgRoomViewComponent))]
	[EntitySystemOf(typeof(DlgRoomViewComponent))]
	public static partial class DlgRoomViewComponentSystem
	{
		[EntitySystem]
		public static void Awake(this DlgRoomViewComponent self)
		{
			self.uiTransform = self.GetParent<UIBaseWindow>().uiTransform;
		}
		[EntitySystem]
		public static void Destroy(this DlgRoomViewComponent self)
		{
			self.DestroyWidget();
		}
	}
}
