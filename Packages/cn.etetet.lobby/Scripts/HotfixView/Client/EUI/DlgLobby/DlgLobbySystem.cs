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
            await EnterMapHelper.EnterMapAsync(root);
            root.GetComponent<UIComponent>().CloseWindow(WindowID.WindowID_Lobby);
        }

        private static void OnMatchClick(EntityRef<DlgLobby> selfRef)
        {
            DlgLobby self = selfRef;
            if (self == null) { return; }
            self.Root().GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_MatchTeam);
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
