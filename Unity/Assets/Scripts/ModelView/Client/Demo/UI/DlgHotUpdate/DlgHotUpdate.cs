namespace ET.Client
{
	 [ComponentOf(typeof(UIBaseWindow))]
	public  class DlgHotUpdate :Entity,IAwake,IUILogic
	{

		public DlgHotUpdateViewComponent View { get => this.GetComponent<DlgHotUpdateViewComponent>();} 

		 

	}
}
