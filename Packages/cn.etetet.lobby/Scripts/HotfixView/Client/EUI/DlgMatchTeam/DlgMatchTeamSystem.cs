using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof(DlgMatchTeam))]
    public static class DlgMatchTeamSystem
    {
        public static void RegisterUIEvent(this DlgMatchTeam self)
        {
            EntityRef<DlgMatchTeam> selfRef = self;
            Log.Warning($"[匹配诊断] DlgMatchTeam.RegisterUIEvent: E_Confirm={(self.View.E_ConfirmButton != null)} E_Cancel={(self.View.E_CancelButton != null)} Loop={(self.View.ELoopScrollList_RolesLoopHorizontalScrollRect != null)}");
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

            // 打开匹配窗口即自动开始匹配(无需确认按钮):倒计时(模拟匹配)结束后请求机器人对手并进 BallBattle
            Log.Warning("[匹配诊断] DlgMatchTeam.ShowWindow -> 自动开始匹配");
            self.IsMatching = true;
            self.StartCountDown().Coroutine();
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
            Log.Info($"[匹配诊断] 开始倒计时 {total}s");
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
            Log.Info("[匹配诊断] 倒计时结束 -> 隐藏窗口, 进 BallBattle(对手=竞技场 NPC 机器人)");

            // 匹配完成自动进入战斗：进球球大作战专属地图(BallBattle Copy)。
            // 对手由服务端 BallArenaComponentSystem.SpawnRobots 在图内生成的 NPC AI 机器人提供。
            // (原 RequestMatchRobotAsync 已移除:其 proto C2G_RequestMatchRobot opcode=20305 在内网区,
            //  会被服务端 NetComponentSystem.OnRead 以"client message must in (10000,20000)"拒收而永久挂起;
            //  且那种机器人是登录进 Map2、不进 BallBattle,对本玩法无用。)
            Scene root = self.Root();
            EntityRef<Scene> rootRef = root;
            root.GetComponent<UIComponent>().HideWindow(WindowID.WindowID_MatchTeam);
            await EnterMapHelper.EnterMapAsync(root, "BallBattle");
            // 进图后关闭大厅:匹配链路原先只 HideWindow(MatchTeam),漏关 Lobby → 进图后大厅仍开着。
            // 与"直接进图"按钮 DlgLobbySystem.OnEnterMap 的 CloseWindow(WindowID_Lobby) 行为一致(隐藏+卸载)。
            root = rootRef;
            root.GetComponent<UIComponent>().CloseWindow(WindowID.WindowID_Lobby);
            Log.Info("[匹配诊断] EnterMapAsync(BallBattle) 返回 + 已关闭大厅");
        }

        private static void OnStartMatchClick(EntityRef<DlgMatchTeam> selfRef)
        {
            DlgMatchTeam self = selfRef;
            Log.Warning($"[匹配诊断] OnStartMatchClick 触发: self={(self != null)} IsMatching={(self != null && self.IsMatching)}");
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
