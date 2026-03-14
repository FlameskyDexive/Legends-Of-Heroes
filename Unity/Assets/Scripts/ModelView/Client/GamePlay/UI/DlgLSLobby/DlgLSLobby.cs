namespace ET.Client
{
	 [ComponentOf(typeof(UIBaseWindow))]
	public  class DlgLSLobby :Entity,IAwake,IUILogic
	{

		public DlgLSLobbyViewComponent View { get => this.GetComponent<DlgLSLobbyViewComponent>();} 

		 

	}
}
