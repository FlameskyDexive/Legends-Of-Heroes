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

        // 匹配倒计时：随机 10-15s。期间可点取消中断；倒计时结束视为匹配成功，进入战斗（状态同步共享地图）。
        // 真实对手不足时，由服务端 GM 命令 matchrobot 创建机器人玩家登录进同一张图当对手。
        [EnableGetComponent(typeof(TimerComponent))]
        public static async ETTask StartCountDown(this DlgMatchTeam self)
        {
            EntityRef<DlgMatchTeam> selfRef = self;
            int total = RandomGenerator.RandomNumber(10, 16); // [10,15] 秒
            for (int remain = total; remain > 0; remain--)
            {
                self = selfRef;
                if (self == null || !self.IsMatching)
                {
                    if (self != null) self.View.ECountDownText.text = string.Empty;
                    return;
                }

                self.View.ECountDownText.text = $"匹配中... {remain}s";
                await self.Root().GetComponent<TimerComponent>().WaitAsync(1000);
            }

            self = selfRef;
            if (self == null || !self.IsMatching) { return; }
            self.IsMatching = false;
            self.View.ECountDownText.text = "匹配成功!";

            // 匹配超时兜底：请求服务端创建一个机器人玩家当对手（机器人会登录并进同一张共享地图）
            Scene root = self.Root();
            await EnterMapHelper.RequestMatchRobotAsync(root);

            // 匹配完成自动进入战斗：状态同步进共享地图，与图内对手（真实玩家或机器人）相遇
            self = selfRef;
            if (self == null) { return; }
            root = self.Root();
            root.GetComponent<UIComponent>().HideWindow(WindowID.WindowID_MatchTeam);
            await EnterMapHelper.EnterMapAsync(root);
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
