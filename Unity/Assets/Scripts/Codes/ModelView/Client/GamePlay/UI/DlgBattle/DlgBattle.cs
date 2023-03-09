namespace ET.Client
{
	 [ComponentOf(typeof(UIBaseWindow))]
	public  class DlgBattle :Entity,IAwake,IUILogic,IUpdate,IDestroy
	{

		public DlgBattleViewComponent View { get => this.GetParent<UIBaseWindow>().GetComponent<DlgBattleViewComponent>();} 

		 

	}
}
