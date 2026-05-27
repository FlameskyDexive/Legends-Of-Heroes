namespace ET.Client
{
    [EntitySystemOf(typeof(DlgRoomListViewComponent))]
    [FriendOf(typeof(DlgRoomListViewComponent))]
    public static partial class DlgRoomListViewComponentSystem
    {
        [EntitySystem]
        private static void Awake(this DlgRoomListViewComponent self)
        {
            self.uiTransform = self.Parent.GetParent<UIBaseWindow>().uiTransform;
        }

        [EntitySystem]
        private static void Destroy(this DlgRoomListViewComponent self)
        {
            self.DestroyWidget();
        }
    }
}
