using System.Collections.Generic;

namespace ET
{
	public  class DlgLobby :Entity
	{

		public DlgLobbyViewComponent View { get => this.Parent.GetComponent<DlgLobbyViewComponent>();}

		public Dictionary<int, Scroll_Item_test> ItemsDictionary;

	}
}
