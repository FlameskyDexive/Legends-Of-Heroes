namespace ET.Client
{
	 [ComponentOf(typeof(UIBaseWindow))]
	public  class DlgRedDot :Entity,IAwake,IUILogic
	{

		public DlgRedDotViewComponent View { get => this.GetComponent<DlgRedDotViewComponent>();} 

		 

	}
}
