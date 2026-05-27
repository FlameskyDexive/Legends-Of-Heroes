namespace ET.Client
{
    [EntitySystemOf(typeof(DlgLSLobbyViewComponent))]
    [FriendOf(typeof(DlgLSLobbyViewComponent))]
    public static partial class DlgLSLobbyViewComponentSystem
    {
        [EntitySystem]
        private static void Awake(this DlgLSLobbyViewComponent self)
        {
            self.uiTransform = self.Parent.GetParent<UIBaseWindow>().uiTransform;
        }

        [EntitySystem]
        private static void Destroy(this DlgLSLobbyViewComponent self)
        {
            self.DestroyWidget();
        }
    }
}
