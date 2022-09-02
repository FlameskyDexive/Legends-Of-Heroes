
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[ObjectSystem]
	public class DlgLoginViewComponentAwakeSystem : AwakeSystem<DlgLoginViewComponent> 
	{
		protected override void Awake(DlgLoginViewComponent self)
		{
			self.uiTransform = self.Parent.GetParent<UIBaseWindow>().uiTransform;
		}
	}


	[ObjectSystem]
	public class DlgLoginViewComponentDestroySystem : DestroySystem<DlgLoginViewComponent> 
	{
		protected override void Destroy(DlgLoginViewComponent self)
		{
			self.DestroyWidget();
		}
	}
}
