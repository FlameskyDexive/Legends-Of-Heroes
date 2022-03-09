namespace ET
{
	public  class DlgHelper :Entity,IAwake,IUILogic
	{

		public DlgHelperViewComponent View { get => this.Parent.GetComponent<DlgHelperViewComponent>();} 

		 

	}
}
