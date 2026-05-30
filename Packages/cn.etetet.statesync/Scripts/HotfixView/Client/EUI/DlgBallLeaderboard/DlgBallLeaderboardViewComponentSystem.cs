namespace ET.Client
{
    [EntitySystemOf(typeof(DlgBallLeaderboardViewComponent))]
    [FriendOf(typeof(DlgBallLeaderboardViewComponent))]
    public static partial class DlgBallLeaderboardViewComponentSystem
    {
        [EntitySystem]
        private static void Awake(this DlgBallLeaderboardViewComponent self)
        {
            self.uiTransform = self.Parent.GetParent<UIBaseWindow>().uiTransform;
        }

        [EntitySystem]
        private static void Destroy(this DlgBallLeaderboardViewComponent self)
        {
            self.DestroyWidget();
        }
    }
}
