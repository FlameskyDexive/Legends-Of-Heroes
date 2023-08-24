
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[FriendOf(typeof(DlgBattleViewComponent))]
	[EntitySystemOf(typeof(DlgBattleViewComponent))]
	public static partial class DlgBattleViewComponentSystem
	{
		[EntitySystem]
		public static void Awake(this DlgBattleViewComponent self)
		{
			self.uiTransform = self.GetParent<UIBaseWindow>().uiTransform;
		}
		[EntitySystem]
		public static void Destroy(this DlgBattleViewComponent self)
		{
			self.DestroyWidget();
		}
	}
}
