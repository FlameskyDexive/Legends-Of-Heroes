
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	
	[FriendOf(typeof(Scroll_Item_role))]
	[EntitySystemOf(typeof(Scroll_Item_role))]
	public static partial class Scroll_Item_roleSystem
	{
		[EntitySystem]
		public static void Awake(this Scroll_Item_role self)
		{
			
		}
		[EntitySystem]
		public static void Destroy(this Scroll_Item_role self)
		{
			self.DestroyWidget();
		}
	}
}
