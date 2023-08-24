
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	
	[FriendOf(typeof(DlgLoginViewComponent))]
	[EntitySystemOf(typeof(DlgLoginViewComponent))]
	public static partial class DlgLoginViewComponentSystem
	{
		[EntitySystem]
		public static void Awake(this DlgLoginViewComponent self)
		{
			self.uiTransform = self.GetParent<UIBaseWindow>().uiTransform;
		}
		[EntitySystem]
		public static void Destroy(this DlgLoginViewComponent self)
		{
			self.DestroyWidget();
		}
	}
}
