using System.Collections.Generic;

namespace ET.Client
{
	 [ComponentOf(typeof(UIBaseWindow))]
	public  class DlgLogin :Entity,IAwake,IUILogic
	{

		public DlgLoginViewComponent View { get => this.GetComponent<DlgLoginViewComponent>();}

		public Dictionary<int, EntityRef<Scroll_Item_test>> Dictionary;

	}
}
