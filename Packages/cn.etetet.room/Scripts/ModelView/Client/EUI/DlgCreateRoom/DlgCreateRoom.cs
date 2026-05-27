namespace ET.Client
{
    [ComponentOf(typeof(UIBaseWindow))]
    public class DlgCreateRoom : Entity, IAwake, IUILogic
    {
        public DlgCreateRoomViewComponent View => this.GetComponent<DlgCreateRoomViewComponent>();
    }
}
