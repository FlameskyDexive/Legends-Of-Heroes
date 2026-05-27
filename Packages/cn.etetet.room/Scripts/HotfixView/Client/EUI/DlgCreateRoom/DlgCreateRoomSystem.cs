using System;

namespace ET.Client
{
    [FriendOf(typeof(DlgCreateRoom))]
    public static partial class DlgCreateRoomSystem
    {
        public static void RegisterUIEvent(this DlgCreateRoom self)
        {
            EntityRef<DlgCreateRoom> selfRef = self;
            self.View.ECreateButton.AddListenerAsync(self.Root(), () => OnCreateClickAsync(selfRef));
            self.View.ECancelButton.AddListener(self.Root(), () => OnCancelClick(selfRef));
        }

        public static void ShowWindow(this DlgCreateRoom self, Entity contextData = null)
        {
            self.View.ERoomNameInputField.text = string.Empty;
            self.View.EModeDropdown.value = 0;
            self.View.EMaxPlayersInputField.text = "2";
            self.View.EPasswordInputField.text = string.Empty;
        }

        private static async ETTask OnCreateClickAsync(EntityRef<DlgCreateRoom> selfRef)
        {
            DlgCreateRoom self = selfRef;
            if (self == null) { return; }

            string roomName = self.View.ERoomNameInputField.text;
            if (string.IsNullOrEmpty(roomName))
            {
                roomName = $"Room{UnityEngine.Random.Range(1000, 9999)}";
            }

            RoomMode mode = (RoomMode)self.View.EModeDropdown.value;

            if (!int.TryParse(self.View.EMaxPlayersInputField.text, out int maxPlayers))
            {
                maxPlayers = 2;
            }
            if (maxPlayers < 2 || maxPlayers > 10)
            {
                maxPlayers = 2;
            }

            string password = self.View.EPasswordInputField.text;

            try
            {
                G2C_CreateRoom response = await EnterMapHelper.CreateRoomAsync(self.Fiber(), roomName, mode, maxPlayers, password);
                self = selfRef;
                if (self == null) { return; }

                if (response.Error == ErrorCode.ERR_Success)
                {
                    self.Root().GetComponent<UIComponent>().HideWindow(WindowID.WindowID_CreateRoom);
                }
                else
                {
                    Log.Error($"Create room failed: {response.Message}");
                }
            }
            catch (Exception e)
            {
                Log.Error($"Create room error: {e}");
            }
        }

        private static void OnCancelClick(EntityRef<DlgCreateRoom> selfRef)
        {
            DlgCreateRoom self = selfRef;
            if (self == null) { return; }
            self.Root().GetComponent<UIComponent>().HideWindow(WindowID.WindowID_CreateRoom);
        }
    }
}
