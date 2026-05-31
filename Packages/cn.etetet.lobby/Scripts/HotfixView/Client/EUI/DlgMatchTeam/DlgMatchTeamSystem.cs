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

        // 匹配到的对手数(模拟,与竞技场服务端机器人数 BallDefine.RobotCount 对应;lobby 不依赖 ballbattle,故此处用常量)
        private const int OpponentCount = 5;
        // 匹配成功后进入战斗前的倒计时(秒)
        private const int MatchReadySeconds = 5;
        // "搜索中"计时器递增的随机时长区间(秒):到点视为匹配到对手
        private const int SearchMinSeconds = 3;
        private const int SearchMaxSeconds = 8;

        public static void ShowWindow(this DlgMatchTeam self, Entity contextData = null)
        {
            self.IsMatching = false;

            // 每次开窗重置成员=仅自己(清掉上次匹配残留的对手),搜索阶段只显示自己。
            self.MemberIds.Clear();
            long myId = self.Root().GetComponent<PlayerComponent>().MyId;
            self.MemberIds.Add(myId);

            self.RefreshMembers();
            self.View.ECountDownText.text = string.Empty;

            // 打开匹配窗口即自动开始匹配(无需确认按钮):搜索(计时器递增)→匹配到对手→5s倒计时→进 BallBattle
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

        // 匹配流程(三阶段):
        // ① 搜索中:计时器从 0 递增(模拟匹配),随机 SearchMin~Max 秒后视为"匹配到对手";
        // ② 匹配到对手:把对手(模拟,数量=OpponentCount)加入成员列表并刷新显示头像/名字,开始 5s 倒计时;
        // ③ 倒计时结束:进球球大作战专属地图(BallBattle Copy),对手由服务端 SpawnRobots 在图内生成的 AI 机器人提供。
        // 期间点取消(IsMatching=false)即中断。
        [EnableGetComponent(typeof(TimerComponent))]
        public static async ETTask StartCountDown(this DlgMatchTeam self)
        {
            EntityRef<DlgMatchTeam> selfRef = self;

            // ① 搜索中:计时器从 0 递增
            int searchSeconds = RandomGenerator.RandomNumber(SearchMinSeconds, SearchMaxSeconds + 1);
            Log.Info($"[匹配诊断] 搜索中(计时器递增), {searchSeconds}s 后匹配到对手");
            for (int elapsed = 0; elapsed < searchSeconds; elapsed++)
            {
                self = selfRef;
                if (self == null || !self.IsMatching)
                {
                    if (self != null) self.View.ECountDownText.text = string.Empty;
                    return;
                }
                self.View.ECountDownText.text = $"匹配中... {elapsed}s";
                await self.Root().GetComponent<TimerComponent>().WaitAsync(1000);
            }

            // ② 匹配到对手:加入对手成员(模拟,仅用于展示头像/名字)并刷新角色列表
            self = selfRef;
            if (self == null || !self.IsMatching) { return; }
            long myId = self.Root().GetComponent<PlayerComponent>().MyId;
            for (int i = 0; i < OpponentCount; i++)
            {
                self.MemberIds.Add(myId + i + 1);
            }
            self.RefreshMembers();
            Log.Info($"[匹配诊断] 匹配到 {OpponentCount} 个对手 -> 刷新角色列表, 开始 {MatchReadySeconds}s 倒计时");

            // ② 倒计时 5s
            for (int remain = MatchReadySeconds; remain > 0; remain--)
            {
                self = selfRef;
                if (self == null || !self.IsMatching) { return; }
                self.View.ECountDownText.text = $"匹配成功! {remain}s 后进入战斗";
                await self.Root().GetComponent<TimerComponent>().WaitAsync(1000);
            }

            // ③ 进入战斗:进球球大作战专属地图(BallBattle Copy)。
            // 对手由服务端 BallArenaComponentSystem.SpawnRobots 在图内生成的 AI 机器人提供。
            self = selfRef;
            if (self == null || !self.IsMatching) { return; }
            self.IsMatching = false;
            Log.Info("[匹配诊断] 倒计时结束 -> 隐藏窗口, 进 BallBattle");

            Scene root = self.Root();
            EntityRef<Scene> rootRef = root;
            root.GetComponent<UIComponent>().HideWindow(WindowID.WindowID_MatchTeam);
            await EnterMapHelper.EnterMapAsync(root, "BallBattle");
            // 进图后关闭大厅(隐藏+卸载),与"直接进图"按钮 DlgLobbySystem.OnEnterMap 行为一致。
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
