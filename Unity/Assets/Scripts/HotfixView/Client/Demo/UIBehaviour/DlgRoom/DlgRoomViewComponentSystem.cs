
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[ObjectSystem]
	public class DlgRoomViewComponentAwakeSystem : AwakeSystem<DlgRoomViewComponent> 
	{
		protected override void Awake(DlgRoomViewComponent self)
		{
			self.uiTransform = self.GetParent<UIBaseWindow>().uiTransform;
		}
	}


	[ObjectSystem]
	public class DlgRoomViewComponentDestroySystem : DestroySystem<DlgRoomViewComponent> 
	{
		protected override void Destroy(DlgRoomViewComponent self)
		{
			self.DestroyWidget();
		}
	}
}
