
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	
	[FriendOf(typeof(DlgLoginViewComponent))]
	[EntitySystemOf(typeof(DlgLoginViewComponent))]
	public static class DlgLoginViewComponentSystem
	{
		public static void Awake(this DlgLoginViewComponent self)
		{
			self.uiTransform = self.GetParent<UIBaseWindow>().uiTransform;
		}
		public static void Destroy(this DlgLoginViewComponent self)
		{
			self.DestroyWidget();
		}
	}
}
