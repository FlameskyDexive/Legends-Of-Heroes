
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[EntitySystemOf(typeof(DlgHotUpdateViewComponent))]
	[FriendOfAttribute(typeof(ET.Client.DlgHotUpdateViewComponent))]
	public static partial class DlgHotUpdateViewComponentSystem
	{
		[EntitySystem]
		private static void Awake(this DlgHotUpdateViewComponent self)
		{
			self.uiTransform = self.Parent.GetParent<UIBaseWindow>().uiTransform;
		}


		[EntitySystem]
		private static void Destroy(this DlgHotUpdateViewComponent self)
		{
			self.DestroyWidget();
		}
	}


}
