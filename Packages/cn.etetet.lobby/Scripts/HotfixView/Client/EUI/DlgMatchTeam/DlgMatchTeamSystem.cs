using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof(DlgMatchTeam))]
    public static class DlgMatchTeamSystem
    {
        public static void RegisterUIEvent(this DlgMatchTeam self)
        {
            EntityRef<DlgMatchTeam> selfRef = self;
            self.View.E_ConfirmButton.AddListener(self.Root(), () => OnStartMatchClick(selfRef));
            self.View.E_CancelButton.AddListener(self.Root(), () => OnCancelClick(selfRef));
            self.View.ELoopScrollList_RolesLoopHorizontalScrollRect.AddItemRefreshListener(self.OnScrollItemRefreshHandler);
        }

        public static void ShowWindow(this DlgMatchTeam self, Entity contextData = null)
        {
            self.IsMatching = false;

            if (self.MemberIds.Count <= 0)
            {
                long myId = self.Root().GetComponent<PlayerComponent>().MyId;
                self.MemberIds.Add(myId);
            }

            self.RefreshMembers();
            self.View.ECountDownText.text = string.Empty;
        }

        public static void OnScrollItemRefreshHandler(this DlgMatchTeam self, Transform transform, int index)
        {
            if (self.ScrollItemRoles == null || !self.ScrollItemRoles.ContainsKey(index))
            {
                return;
            }

            Scroll_Item_role itemRole = self.ScrollItemRoles[index].BindTrans(transform);

            if (self.MemberIds.Count > index)
            {
                long memberId = self.MemberIds[index];
                itemRole.E_RoleNameText.text = memberId.ToString();
                int avatarIndex = (index % 9) + 1;
                itemRole.E_AvatarImage.sprite = self.Root().GetComponent<ResourcesLoaderComponent>().LoadAssetSync<Sprite>($"Avatar{avatarIndex}");
            }
        }

        public static void RefreshMembers(this DlgMatchTeam self)
        {
            int count = self.MemberIds.Count;
            self.RemoveUIScrollItems(ref self.ScrollItemRoles);
            self.AddUIScrollItems(ref self.ScrollItemRoles, count);
            self.View.ELoopScrollList_RolesLoopHorizontalScrollRect.SetVisible(true, count);
        }

        public static async ETTask StartCountDown(this DlgMatchTeam self)
        {
            EntityRef<DlgMatchTeam> selfRef = self;
            for (int i = 5; i > 0; i--)
            {
                self = selfRef;
                if (self == null || !self.IsMatching)
                {
                    if (self != null) self.View.ECountDownText.text = string.Empty;
                    return;
                }

                self.View.ECountDownText.text = i.ToString();
                await self.Root().GetComponent<TimerComponent>().WaitAsync(1000);
            }

            self = selfRef;
            if (self == null) { return; }
            self.View.ECountDownText.text = string.Empty;
            await self.Root().GetComponent<TimerComponent>().WaitAsync(500);
            self = selfRef;
            if (self == null) { return; }
            self.IsMatching = false;
        }

        private static void OnStartMatchClick(EntityRef<DlgMatchTeam> selfRef)
        {
            DlgMatchTeam self = selfRef;
            if (self == null || self.IsMatching) { return; }
            self.IsMatching = true;
            self.StartCountDown().Coroutine();
        }

        private static void OnCancelClick(EntityRef<DlgMatchTeam> selfRef)
        {
            DlgMatchTeam self = selfRef;
            if (self == null) { return; }
            self.IsMatching = false;
            self.Root().GetComponent<UIComponent>().HideWindow(WindowID.WindowID_MatchTeam);
        }
    }
}
