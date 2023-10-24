namespace ET.Client
{
	 [ComponentOf(typeof(UIBaseWindow))]
	public  class DlgLobby :Entity,IAwake,IUILogic
	{

		public DlgLobbyViewComponent View { get => this.GetComponent<DlgLobbyViewComponent>();} 

		 

	}
}
