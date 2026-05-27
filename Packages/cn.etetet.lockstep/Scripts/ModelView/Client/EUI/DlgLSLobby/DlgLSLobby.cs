namespace ET.Client
{
    [ComponentOf(typeof(UIBaseWindow))]
    public class DlgLSLobby : Entity, IAwake, IUILogic
    {
        public DlgLSLobbyViewComponent View => this.GetComponent<DlgLSLobbyViewComponent>();
    }
}
