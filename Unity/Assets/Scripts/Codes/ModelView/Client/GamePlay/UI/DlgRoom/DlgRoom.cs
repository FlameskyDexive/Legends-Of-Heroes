using System.Collections.Generic;

namespace ET.Client
{
	 [ComponentOf(typeof(UIBaseWindow))]
	public  class DlgRoom :Entity,IAwake,IUILogic, IUpdate, IDestroy
	{

		public DlgRoomViewComponent View { get => this.GetParent<UIBaseWindow>().GetComponent<DlgRoomViewComponent>();}

        public Dictionary<int, Scroll_Item_role> ScrollItemRoles;

    }
}
