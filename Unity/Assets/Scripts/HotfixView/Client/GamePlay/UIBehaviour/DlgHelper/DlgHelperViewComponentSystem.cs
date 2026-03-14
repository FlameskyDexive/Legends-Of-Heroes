
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[EntitySystemOf(typeof(DlgHelperViewComponent))]
	[FriendOfAttribute(typeof(ET.Client.DlgHelperViewComponent))]
	public static partial class DlgHelperViewComponentSystem
	{
		[EntitySystem]
		private static void Awake(this DlgHelperViewComponent self)
		{
			self.uiTransform = self.Parent.GetParent<UIBaseWindow>().uiTransform;
		}


		[EntitySystem]
		private static void Destroy(this DlgHelperViewComponent self)
		{
			self.DestroyWidget();
		}
	}


}
