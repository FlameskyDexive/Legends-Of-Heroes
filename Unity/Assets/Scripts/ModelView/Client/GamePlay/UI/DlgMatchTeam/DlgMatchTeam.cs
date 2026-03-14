using System.Collections.Generic;

namespace ET.Client
{
	 [ComponentOf(typeof(UIBaseWindow))]
	public  class DlgMatchTeam :Entity,IAwake,IUILogic
	{

		public DlgMatchTeamViewComponent View { get => this.GetComponent<DlgMatchTeamViewComponent>();}

        public Dictionary<int, EntityRef<Scroll_Item_role>> ScrollItemRoles;

        public List<long> MemberIds = new List<long>();

        public bool IsMatching;

    }
}

