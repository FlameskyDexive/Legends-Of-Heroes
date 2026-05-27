namespace ET.Client
{
    [EntitySystemOf(typeof(DlgRoomViewComponent))]
    [FriendOf(typeof(DlgRoomViewComponent))]
    public static partial class DlgRoomViewComponentSystem
    {
        [EntitySystem]
        private static void Awake(this DlgRoomViewComponent self)
        {
            self.uiTransform = self.Parent.GetParent<UIBaseWindow>().uiTransform;
        }

        [EntitySystem]
        private static void Destroy(this DlgRoomViewComponent self)
        {
            self.DestroyWidget();
        }
    }
}
