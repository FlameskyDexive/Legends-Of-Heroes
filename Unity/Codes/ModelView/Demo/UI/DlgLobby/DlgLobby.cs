using System.Collections.Generic;

namespace ET
{
	public  class DlgLobby :Entity,IAwake,IDestroy
	{

		public DlgLobbyViewComponent View { get => this.Parent.GetComponent<DlgLobbyViewComponent>();}

	

	}
}
