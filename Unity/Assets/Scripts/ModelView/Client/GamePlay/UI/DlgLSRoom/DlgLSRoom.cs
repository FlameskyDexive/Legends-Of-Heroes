namespace ET.Client
{
	 [ComponentOf(typeof(UIBaseWindow))]
	public  class DlgLSRoom :Entity,IAwake,IUILogic,IUpdate
	{

		public DlgLSRoomViewComponent View { get => this.GetComponent<DlgLSRoomViewComponent>();} 

		public int frame;
		public int predictFrame;

	}
}
