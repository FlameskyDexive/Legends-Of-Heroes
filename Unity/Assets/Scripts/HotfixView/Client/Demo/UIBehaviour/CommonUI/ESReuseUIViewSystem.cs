
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[EntitySystemOf(typeof(ESReuseUI))]
	[FriendOfAttribute(typeof(ESReuseUI))]
	public static partial class ESReuseUISystem 
	{
		[EntitySystem]
		private static void Awake(this ESReuseUI self,Transform transform)
		{
			self.uiTransform = transform;
		}

		[EntitySystem]
		private static void Destroy(this ESReuseUI self)
		{
			self.DestroyWidget();
		}
	}


}
