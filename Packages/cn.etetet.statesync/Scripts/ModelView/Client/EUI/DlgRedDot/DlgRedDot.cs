namespace ET.Client
{
    [ComponentOf(typeof(UIBaseWindow))]
    public class DlgRedDot : Entity, IAwake, IUILogic
    {
        public DlgRedDotViewComponent View => this.GetComponent<DlgRedDotViewComponent>();
    }
}
