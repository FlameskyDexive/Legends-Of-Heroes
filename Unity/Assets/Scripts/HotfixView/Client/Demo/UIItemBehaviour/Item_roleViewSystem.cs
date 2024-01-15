
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[EntitySystemOf(typeof(Scroll_Item_role))]
	public static partial class Scroll_Item_roleSystem 
	{
		[EntitySystem]
		private static void Awake(this Scroll_Item_role self )
		{
		}

		[EntitySystem]
		private static void Destroy(this Scroll_Item_role self )
		{
			self.DestroyWidget();
		}
	}
}
