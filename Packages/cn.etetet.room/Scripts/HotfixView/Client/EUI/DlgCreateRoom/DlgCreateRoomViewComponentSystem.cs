namespace ET.Client
{
    [EntitySystemOf(typeof(DlgCreateRoomViewComponent))]
    [FriendOf(typeof(DlgCreateRoomViewComponent))]
    public static partial class DlgCreateRoomViewComponentSystem
    {
        [EntitySystem]
        private static void Awake(this DlgCreateRoomViewComponent self)
        {
            self.uiTransform = self.Parent.GetParent<UIBaseWindow>().uiTransform;
        }

        [EntitySystem]
        private static void Destroy(this DlgCreateRoomViewComponent self)
        {
            self.DestroyWidget();
        }
    }
}
