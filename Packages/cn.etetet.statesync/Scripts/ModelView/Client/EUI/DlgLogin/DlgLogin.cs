namespace ET.Client
{
    [ComponentOf(typeof(UIBaseWindow))]
    public class DlgLogin : Entity, IAwake, IUILogic
    {
        public DlgLoginViewComponent View => this.GetComponent<DlgLoginViewComponent>();
    }
}
