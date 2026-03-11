
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[EntitySystemOf(typeof(DlgMatchTeamViewComponent))]
	[FriendOfAttribute(typeof(ET.Client.DlgMatchTeamViewComponent))]
	public static partial class DlgMatchTeamViewComponentSystem
	{
		[EntitySystem]
		private static void Awake(this DlgMatchTeamViewComponent self)
		{
			self.uiTransform = self.Parent.GetParent<UIBaseWindow>().uiTransform;
		}


		[EntitySystem]
		private static void Destroy(this DlgMatchTeamViewComponent self)
		{
			self.DestroyWidget();
		}
	}


}

