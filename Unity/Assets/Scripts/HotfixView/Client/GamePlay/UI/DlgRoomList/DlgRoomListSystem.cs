using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof(DlgRoomList))]
    public static partial class DlgRoomListSystem
    {
        public static void RegisterUIEvent(this DlgRoomList self)
        {
            self.View.ERefreshButton.AddListenerAsync(self.Root(), self.OnRefreshClick);
            self.View.ECreateRoomButton.AddListener(self.Root(), self.OnCreateRoomClick);
            self.View.EBackButton.AddListener(self.Root(), self.OnBackClick);
        }

        public static void ShowWindow(this DlgRoomList self, Entity contextData = null)
        {
            self.RefreshRoomList().Coroutine();
        }

        public static async ETTask OnRefreshClick(this DlgRoomList self)
        {
            await self.RefreshRoomList();
        }

        public static void OnCreateRoomClick(this DlgRoomList self)
        {
            self.Root().GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_CreateRoom);
        }

        public static void OnBackClick(this DlgRoomList self)
        {
            self.Root().GetComponent<UIComponent>().HideWindow(WindowID.WindowID_RoomList);
        }

        public static async ETTask RefreshRoomList(this DlgRoomList self)
        {
            try
            {
                G2C_GetRoomList response = await EnterMapHelper.GetRoomListAsync(self.Fiber(), null);
                if (response.Error == ErrorCode.ERR_Success)
                {
                    self.DisplayRoomList(response.RoomList);
                }
                else
                {
                    Log.Error($"Get room list failed: {response.Message}");
                }
            }
            catch (Exception e)
            {
                Log.Error($"Refresh room list error: {e}");
            }
        }

        public static void DisplayRoomList(this DlgRoomList self, List<RoomInfo> roomList)
        {
            foreach (Transform child in self.View.ERoomListContentRectTransform)
            {
                UnityEngine.Object.Destroy(child.gameObject);
            }

            foreach (RoomInfo roomInfo in roomList)
            {
                GameObject roomItem = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/RoomItem"));
                roomItem.transform.SetParent(self.View.ERoomListContentRectTransform);
                roomItem.transform.localScale = Vector3.one;

                RoomItemComponent roomItemComponent = roomItem.GetComponent<RoomItemComponent>();
                if (roomItemComponent != null)
                {
                    roomItemComponent.SetRoomInfo(roomInfo);
                    roomItemComponent.OnJoinClick = async () => await self.OnJoinRoomClick(roomInfo.RoomId, roomInfo.Password);
                }
            }
        }

        public static async ETTask OnJoinRoomClick(this DlgRoomList self, long roomId, string password)
        {
            try
            {
                G2C_JoinRoom response = await EnterMapHelper.JoinRoomAsync(self.Fiber(), roomId, password);
                if (response.Error == ErrorCode.ERR_Success)
                {
                    Log.Debug($"Join room success: {response.RoomInfo.RoomName}");
                    self.Root().GetComponent<UIComponent>().HideWindow(WindowID.WindowID_RoomList);
                    self.Root().GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_Room);
                }
                else
                {
                    Log.Error($"Join room failed: {response.Message}");
                }
            }
            catch (Exception e)
            {
                Log.Error($"Join room error: {e}");
            }
        }
    }
}