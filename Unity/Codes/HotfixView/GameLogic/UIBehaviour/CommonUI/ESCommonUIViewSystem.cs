
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	[ObjectSystem]
	public class ESCommonUIAwakeSystem : AwakeSystem<ESCommonUI,Transform> 
	{
		public override void Awake(ESCommonUI self,Transform transform)
		{
			self.uiTransform = transform;
		}
	}


	[ObjectSystem]
	public class ESCommonUIDestroySystem : DestroySystem<ESCommonUI> 
	{
		public override void Destroy(ESCommonUI self)
		{
			self.m_EImage_Test1 = null;
			self.m_ELabel_Test2 = null;
			self.uiTransform = null;
		}
	}
}
