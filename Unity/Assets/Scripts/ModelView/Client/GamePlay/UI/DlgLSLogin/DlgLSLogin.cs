namespace ET.Client
{
	 [ComponentOf(typeof(UIBaseWindow))]
	public  class DlgLSLogin :Entity,IAwake,IUILogic
	{

		public DlgLSLoginViewComponent View { get => this.GetComponent<DlgLSLoginViewComponent>();} 

		 

	}
}
