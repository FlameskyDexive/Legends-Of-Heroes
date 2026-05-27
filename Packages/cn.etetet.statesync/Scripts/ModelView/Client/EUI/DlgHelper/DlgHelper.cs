namespace ET.Client
{
    [ComponentOf(typeof(UIBaseWindow))]
    public class DlgHelper : Entity, IAwake, IUILogic
    {
        public DlgHelperViewComponent View => this.GetComponent<DlgHelperViewComponent>();
    }
}
