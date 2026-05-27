namespace ET.Client
{
    [EntitySystemOf(typeof(DlgRedDotViewComponent))]
    [FriendOf(typeof(DlgRedDotViewComponent))]
    public static partial class DlgRedDotViewComponentSystem
    {
        [EntitySystem]
        private static void Awake(this DlgRedDotViewComponent self)
        {
            self.uiTransform = self.Parent.GetParent<UIBaseWindow>().uiTransform;
        }

        [EntitySystem]
        private static void Destroy(this DlgRedDotViewComponent self)
        {
            self.DestroyWidget();
        }
    }
}
