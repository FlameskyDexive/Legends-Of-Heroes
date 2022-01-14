
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	[ObjectSystem]
	public class DlgLoginViewComponentAwakeSystem : AwakeSystem<DlgLoginViewComponent> 
	{
		public override void Awake(DlgLoginViewComponent self)
		{
			self.uiTransform = self.GetParent<UIBaseWindow>().uiTransform;
		}
	}


	[ObjectSystem]
	public class DlgLoginViewComponentDestroySystem : DestroySystem<DlgLoginViewComponent> 
	{
		public override void Destroy(DlgLoginViewComponent self)
		{
			self.DestroyWidget();
		}
	}
}
