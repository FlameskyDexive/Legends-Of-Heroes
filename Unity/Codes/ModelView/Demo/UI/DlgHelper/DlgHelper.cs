namespace ET
{
	public  class DlgHelper :Entity,IAwake
	{

		public DlgHelperViewComponent View { get => this.Parent.GetComponent<DlgHelperViewComponent>();} 

		 

	}
}
