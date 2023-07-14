
using UnityEngine;
using UnityEngine.UI;
namespace ET.Client
{
	[ObjectSystem]
	public class Scroll_Item_roleDestroySystem : DestroySystem<Scroll_Item_role> 
	{
		protected override void Destroy( Scroll_Item_role self )
		{
			self.DestroyWidget();
		}
	}
}
