namespace ET.Client
{
    [ComponentOf(typeof(UIBaseWindow))]
    public class DlgLobby : Entity, IAwake, IUILogic
    {
        public DlgLobbyViewComponent View => this.GetComponent<DlgLobbyViewComponent>();
    }
}
