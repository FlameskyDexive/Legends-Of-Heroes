namespace ET.Client
{
	[FriendOf(typeof(WindowCoreData))]
	[FriendOf(typeof(UIBaseWindow))]
	[AUIEvent(WindowID.WindowID_Battle)]
	public  class DlgBattleEventHandler : IAUIEventHandler
	{

		public void OnInitWindowCoreData(UIBaseWindow uiBaseWindow)
		{
		  uiBaseWindow.WindowData.windowType = UIWindowType.Normal; 
		}

		public void OnInitComponent(UIBaseWindow uiBaseWindow)
		{
		  uiBaseWindow.AddComponent<DlgBattleViewComponent>(); 
		  uiBaseWindow.AddComponent<DlgBattle>(); 
		}

		public void OnRegisterUIEvent(UIBaseWindow uiBaseWindow)
		{
		  uiBaseWindow.GetComponent<DlgBattle>().RegisterUIEvent(); 
		}

		public void OnShowWindow(UIBaseWindow uiBaseWindow, ShowWindowDataBase contextData = null)
		{
		  uiBaseWindow.GetComponent<DlgBattle>().ShowWindow(contextData); 
		}

		public void OnHideWindow(UIBaseWindow uiBaseWindow)
		{
		}

		public void BeforeUnload(UIBaseWindow uiBaseWindow)
		{
		}

	}
}
