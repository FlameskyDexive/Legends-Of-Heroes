
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[EntitySystemOf(typeof(DlgLSLoginViewComponent))]
	[FriendOfAttribute(typeof(ET.Client.DlgLSLoginViewComponent))]
	public static partial class DlgLSLoginViewComponentSystem
	{
		[EntitySystem]
		private static void Awake(this DlgLSLoginViewComponent self)
		{
			self.uiTransform = self.Parent.GetParent<UIBaseWindow>().uiTransform;
		}


		[EntitySystem]
		private static void Destroy(this DlgLSLoginViewComponent self)
		{
			self.DestroyWidget();
		}
	}


}
