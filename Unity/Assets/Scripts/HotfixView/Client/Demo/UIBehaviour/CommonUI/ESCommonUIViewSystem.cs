
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[EntitySystemOf(typeof(ESCommonUI))]
	[FriendOfAttribute(typeof(ESCommonUI))]
	public static partial class ESCommonUISystem 
	{
		[EntitySystem]
		private static void Awake(this ESCommonUI self,Transform transform)
		{
			self.uiTransform = transform;
		}

		[EntitySystem]
		private static void Destroy(this ESCommonUI self)
		{
			self.DestroyWidget();
		}
	}


}
