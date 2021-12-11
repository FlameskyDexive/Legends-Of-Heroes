using System.Collections.Generic;

namespace ET
{
	public  class DlgTest :Entity
	{

		public DlgTestViewComponent View { get => this.Parent.GetComponent<DlgTestViewComponent>();}

		public Dictionary<int, Scroll_Item_serverTest> ScrollItemServerTestsDict;

	}
}
