
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[EntitySystemOf(typeof(Scroll_Item_test))]
	public static partial class Scroll_Item_testSystem 
	{
		[EntitySystem]
		private static void Awake(this Scroll_Item_test self )
		{
		}

		[EntitySystem]
		private static void Destroy(this Scroll_Item_test self )
		{
			self.DestroyWidget();
		}
	}
}
