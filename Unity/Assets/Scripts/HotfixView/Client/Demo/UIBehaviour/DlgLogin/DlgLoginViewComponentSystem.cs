
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
			Log.Info($"11111:{self.uiTransform?.name}");
		}

        [EntitySystem]
        public static void Destroy(this DlgLoginViewComponent self)
		{
		 
			self.DestroyWidget();
		}

	}
}
