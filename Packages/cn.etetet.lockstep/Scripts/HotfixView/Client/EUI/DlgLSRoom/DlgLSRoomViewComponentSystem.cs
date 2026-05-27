namespace ET.Client
{
    [EntitySystemOf(typeof(DlgLSRoomViewComponent))]
    [FriendOf(typeof(DlgLSRoomViewComponent))]
    public static partial class DlgLSRoomViewComponentSystem
    {
        [EntitySystem]
        private static void Awake(this DlgLSRoomViewComponent self)
        {
            self.uiTransform = self.Parent.GetParent<UIBaseWindow>().uiTransform;
        }

        [EntitySystem]
        private static void Destroy(this DlgLSRoomViewComponent self)
        {
            self.DestroyWidget();
        }
    }
}
