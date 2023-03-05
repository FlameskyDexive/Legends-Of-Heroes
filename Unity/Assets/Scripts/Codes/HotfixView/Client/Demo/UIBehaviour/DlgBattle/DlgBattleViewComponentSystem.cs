
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[ObjectSystem]
	public class DlgBattleViewComponentAwakeSystem : AwakeSystem<DlgBattleViewComponent> 
	{
		protected override void Awake(DlgBattleViewComponent self)
		{
			self.uiTransform = self.GetParent<UIBaseWindow>().uiTransform;
		}
	}


	[ObjectSystem]
	public class DlgBattleViewComponentDestroySystem : DestroySystem<DlgBattleViewComponent> 
	{
		protected override void Destroy(DlgBattleViewComponent self)
		{
			self.DestroyWidget();
		}
	}
}
