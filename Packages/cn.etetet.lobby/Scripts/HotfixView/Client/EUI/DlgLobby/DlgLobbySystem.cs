namespace ET.Client
{
    [FriendOf(typeof(DlgLobby))]
    public static class DlgLobbySystem
    {
        public static void RegisterUIEvent(this DlgLobby self)
        {
            EntityRef<DlgLobby> selfRef = self;
            self.View.EEnterMapButton.AddListenerAsync(self.Root(), () => OnEnterMap(selfRef));
            self.View.EMatchButton.AddListener(self.Root(), () => OnMatchClick(selfRef));
            self.View.ECreateRoomButton.AddListener(self.Root(), () => OnCreateRoomClick(selfRef));
            self.View.ERoomListButton.AddListener(self.Root(), () => OnRoomListClick(selfRef));
        }

        public static void ShowWindow(this DlgLobby self, Entity contextData = null)
        {
        }

        private static async ETTask OnEnterMap(EntityRef<DlgLobby> selfRef)
        {
            DlgLobby self = selfRef;
            if (self == null) { return; }
            Scene root = self.Root();
            EntityRef<Scene> rootRef = root;
            await EnterMapHelper.EnterMapAsync(root);
            root = rootRef;
            root.GetComponent<UIComponent>().CloseWindow(WindowID.WindowID_Lobby);
        }

        private static void OnMatchClick(EntityRef<DlgLobby> selfRef)
        {
            DlgLobby self = selfRef;
            Log.Warning($"[匹配诊断] 大厅 OnMatchClick 触发: self={(self != null)}");
            if (self == null) { return; }
            try
            {
                self.Root().GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_MatchTeam);
                Log.Warning("[匹配诊断] 大厅 ShowWindow(MatchTeam) 已调用");
            }
            catch (System.Exception e)
            {
                Log.Error($"[匹配诊断] ShowWindow(MatchTeam) 异常: {e}");
            }
        }

        private static void OnCreateRoomClick(EntityRef<DlgLobby> selfRef)
        {
            DlgLobby self = selfRef;
            if (self == null) { return; }
            self.Root().GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_CreateRoom);
        }

        private static void OnRoomListClick(EntityRef<DlgLobby> selfRef)
        {
            DlgLobby self = selfRef;
            if (self == null) { return; }
            self.Root().GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_RoomList);
        }
    }
}
