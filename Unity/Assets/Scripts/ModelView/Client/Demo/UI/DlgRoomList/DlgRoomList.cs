namespace ET.Client
{
    [ComponentOf(typeof(UIBaseWindow))]
    public class DlgRoomList : Entity, IAwake, IUILogic
    {
        public DlgRoomListViewComponent View { get => this.GetComponent<DlgRoomListViewComponent>(); }
    }
}