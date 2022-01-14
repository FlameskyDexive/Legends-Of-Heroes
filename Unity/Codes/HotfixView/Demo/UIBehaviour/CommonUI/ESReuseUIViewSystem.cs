
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	[ObjectSystem]
	public class ESReuseUIAwakeSystem : AwakeSystem<ESReuseUI,Transform> 
	{
		public override void Awake(ESReuseUI self,Transform transform)
		{
			self.uiTransform = transform;
		}
	}


	[ObjectSystem]
	public class ESReuseUIDestroySystem : DestroySystem<ESReuseUI> 
	{
		public override void Destroy(ESReuseUI self)
		{
			self.DestroyWidget();
		}
	}
}
