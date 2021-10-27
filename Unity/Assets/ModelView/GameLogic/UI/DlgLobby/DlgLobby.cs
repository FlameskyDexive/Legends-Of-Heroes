using System.Collections.Generic;

namespace ET
{
	public  class DlgLobby :Entity
	{

		public DlgLobbyViewComponent View { get => this.Parent.GetComponent<DlgLobbyViewComponent>();}

	

	}
}
