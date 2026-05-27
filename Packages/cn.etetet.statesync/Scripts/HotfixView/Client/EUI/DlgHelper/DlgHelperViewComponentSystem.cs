namespace ET.Client
{
    [EntitySystemOf(typeof(DlgHelperViewComponent))]
    [FriendOf(typeof(DlgHelperViewComponent))]
    public static partial class DlgHelperViewComponentSystem
    {
        [EntitySystem]
        private static void Awake(this DlgHelperViewComponent self)
        {
            self.uiTransform = self.Parent.GetParent<UIBaseWindow>().uiTransform;
        }

        [EntitySystem]
        private static void Destroy(this DlgHelperViewComponent self)
        {
            self.DestroyWidget();
        }
    }
}
