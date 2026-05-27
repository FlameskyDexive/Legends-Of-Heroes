namespace ET.Client
{
    [EntitySystemOf(typeof(DlgLoginViewComponent))]
    [FriendOf(typeof(DlgLoginViewComponent))]
    public static partial class DlgLoginViewComponentSystem
    {
        [EntitySystem]
        private static void Awake(this DlgLoginViewComponent self)
        {
            self.uiTransform = self.Parent.GetParent<UIBaseWindow>().uiTransform;
        }

        [EntitySystem]
        private static void Destroy(this DlgLoginViewComponent self)
        {
            self.DestroyWidget();
        }
    }
}
