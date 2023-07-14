
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[ObjectSystem]
	public class DlgHotUpdateViewComponentAwakeSystem : AwakeSystem<DlgHotUpdateViewComponent> 
	{
		protected override void Awake(DlgHotUpdateViewComponent self)
		{
			self.uiTransform = self.GetParent<UIBaseWindow>().uiTransform;
		}
	}


	[ObjectSystem]
	public class DlgHotUpdateViewComponentDestroySystem : DestroySystem<DlgHotUpdateViewComponent> 
	{
		protected override void Destroy(DlgHotUpdateViewComponent self)
		{
			self.DestroyWidget();
		}
	}
}
