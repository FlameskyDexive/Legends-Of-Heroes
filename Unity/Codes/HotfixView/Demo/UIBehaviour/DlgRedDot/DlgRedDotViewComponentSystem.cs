
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	[ObjectSystem]
	public class DlgRedDotViewComponentAwakeSystem : AwakeSystem<DlgRedDotViewComponent> 
	{
		public override void Awake(DlgRedDotViewComponent self)
		{
			self.uiTransform = self.GetParent<UIBaseWindow>().uiTransform;
		}
	}


	[ObjectSystem]
	public class DlgRedDotViewComponentDestroySystem : DestroySystem<DlgRedDotViewComponent> 
	{
		public override void Destroy(DlgRedDotViewComponent self)
		{
			self.DestroyWidget();
		}
	}
}
