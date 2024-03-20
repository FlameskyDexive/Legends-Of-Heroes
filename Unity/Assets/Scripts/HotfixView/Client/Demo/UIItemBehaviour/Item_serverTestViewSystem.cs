
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[EntitySystemOf(typeof(Scroll_Item_serverTest))]
	public static partial class Scroll_Item_serverTestSystem 
	{
		[EntitySystem]
		private static void Awake(this Scroll_Item_serverTest self )
		{
		}

		[EntitySystem]
		private static void Destroy(this Scroll_Item_serverTest self )
		{
			self.DestroyWidget();
		}
	}
}
