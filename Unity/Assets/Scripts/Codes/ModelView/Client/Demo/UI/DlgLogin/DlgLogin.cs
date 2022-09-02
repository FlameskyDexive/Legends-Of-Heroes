using System.Collections.Generic;

namespace ET.Client
{
	 [ComponentOf(typeof(UIBaseWindow))]
	public  class DlgLogin :Entity,IAwake,IUILogic
	{

		public DlgLoginViewComponent View { get => this.GetComponent<DlgLoginViewComponent>();}

		public Dictionary<int, Scroll_Item_serverTest> ScrollItemServerTests;

	}
}
