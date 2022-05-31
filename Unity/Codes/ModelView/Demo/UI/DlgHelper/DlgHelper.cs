namespace ET
{
	 [ComponentOf(typeof(UIBaseWindow))]
	public  class DlgHelper :Entity,IAwake,IUILogic
	{

		public DlgHelperViewComponent View { get => this.Parent.GetComponent<DlgHelperViewComponent>();} 

		 

	}
}
