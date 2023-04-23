namespace ET.Client
{
	 [ComponentOf(typeof(UIBaseWindow))]
	public  class DlgBattle :Entity,IAwake,IUILogic,IUpdate,IDestroy
	{

		 public DlgBattleViewComponent View { get => this.GetParent<UIBaseWindow>().GetComponent<DlgBattleViewComponent>();} 

		 public Skill Skill1 { get; set; }
		 public Skill Skill2 { get; set; }
         
         public Unit MyUnit { get; set; }

	}
}
