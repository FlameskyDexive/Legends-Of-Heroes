
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[FriendOf(typeof(DlgBattleViewComponent))]
	[EntitySystemOf(typeof(DlgBattleViewComponent))]
	public static class DlgBattleViewComponentSystem
	{
		public static void Awake(this DlgBattleViewComponent self)
		{
			self.uiTransform = self.GetParent<UIBaseWindow>().uiTransform;
		}
		public static void Destroy(this DlgBattleViewComponent self)
		{
			self.DestroyWidget();
		}
	}
}
