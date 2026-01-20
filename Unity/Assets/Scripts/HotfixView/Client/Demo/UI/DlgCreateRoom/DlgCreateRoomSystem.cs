using System;
using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof(DlgCreateRoom))]
    public static partial class DlgCreateRoomSystem
    {
        public static void RegisterUIEvent(this DlgCreateRoom self)
        {
            self.View.ECreateButton.AddListenerAsync(self.Root(), self.OnCreateClick);
            self.View.ECancelButton.AddListener(self.Root(), self.OnCancelClick);
        }

        public static void ShowWindow(this DlgCreateRoom self, Entity contextData = null)
        {
            self.View.ERoomNameInputField.text = "";
            self.View.EModeDropdown.value = 0;
            self.View.EMaxPlayersInputField.text = "2";
            self.View.EPasswordInputField.text = "";
        }

        public static async ETTask OnCreateClick(this DlgCreateRoom self)
        {
            string roomName = self.View.ERoomNameInputField.text;
            if (string.IsNullOrEmpty(roomName))
            {
                roomName = $"Room{UnityEngine.Random.Range(1000, 9999)}";
            }

            RoomMode mode = (RoomMode)self.View.EModeDropdown.value;

            int maxPlayers;
            if (!int.TryParse(self.View.EMaxPlayersInputField.text, out maxPlayers))
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

        public static void OnCancelClick(this DlgCreateRoom self)
        {
            self.Root().GetComponent<UIComponent>().HideWindow(WindowID.WindowID_CreateRoom);
        }
    }
}