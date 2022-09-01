
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[ObjectSystem]
	public class DlgHelperViewComponentAwakeSystem : AwakeSystem<DlgHelperViewComponent> 
	{
		protected override void Awake(DlgHelperViewComponent self)
		{
			self.uiTransform = self.GetParent<UIBaseWindow>().uiTransform;
		}
	}


	[ObjectSystem]
	public class DlgHelperViewComponentDestroySystem : DestroySystem<DlgHelperViewComponent> 
	{
		protected override void Destroy(DlgHelperViewComponent self)
		{
			self.DestroyWidget();
		}
	}
}
