
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	[ObjectSystem]
	public class DlgHelperViewComponentAwakeSystem : AwakeSystem<DlgHelperViewComponent> 
	{
		public override void Awake(DlgHelperViewComponent self)
		{
			self.uiTransform = self.GetParent<UIBaseWindow>().uiTransform;
		}
	}


	[ObjectSystem]
	public class DlgHelperViewComponentDestroySystem : DestroySystem<DlgHelperViewComponent> 
	{
		public override void Destroy(DlgHelperViewComponent self)
		{
			self.DestroyWidget();
		}
	}
}
