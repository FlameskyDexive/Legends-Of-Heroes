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
            EntityRef<DlgRoomList> selfRef = self;
            self.View.ERefreshButton.AddListenerAsync(self.Root(), () => OnRefreshClickAsync(selfRef));
            self.View.ECreateRoomButton.AddListener(self.Root(), () => OnCreateRoomClick(selfRef));
            self.View.EBackButton.AddListener(self.Root(), () => OnBackClick(selfRef));
        }

        public static void ShowWindow(this DlgRoomList self, Entity contextData = null)
        {
            self.RefreshRoomList().Coroutine();
        }

        public static async ETTask RefreshRoomList(this DlgRoomList self)
        {
            EntityRef<DlgRoomList> selfRef = self;
            try
            {
                G2C_GetRoomList response = await EnterMapHelper.GetRoomListAsync(self.Fiber(), null);
                self = selfRef;
                if (self == null) { return; }
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

            EntityRef<DlgRoomList> selfRef = self;
            foreach (RoomInfo roomInfo in roomList)
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/UI/RoomItem");
                if (prefab == null)
                {
                    Log.Warning("Resources Prefabs/UI/RoomItem not found; skip building list item.");
                    continue;
                }
                GameObject roomItem = UnityEngine.Object.Instantiate(prefab);
                roomItem.transform.SetParent(self.View.ERoomListContentRectTransform);
                roomItem.transform.localScale = Vector3.one;

                RoomItemComponent roomItemComponent = roomItem.GetComponent<RoomItemComponent>();
                if (roomItemComponent != null)
                {
                    roomItemComponent.SetRoomInfo(roomInfo);
                    RoomInfo captured = roomInfo;
                    roomItemComponent.OnJoinClick = () => OnJoinRoomClick(selfRef, captured.RoomId, captured.Password).Coroutine();
                }
            }
        }

        public static async ETTask OnJoinRoomClick(EntityRef<DlgRoomList> selfRef, long roomId, string password)
        {
            DlgRoomList self = selfRef;
            if (self == null) { return; }
            try
            {
                G2C_JoinRoom response = await EnterMapHelper.JoinRoomAsync(self.Fiber(), roomId, password);
                self = selfRef;
                if (self == null) { return; }
                if (response.Error == ErrorCode.ERR_Success)
                {
                    Log.Debug($"Join room success: {response.RoomInfo?.RoomName}");
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

        private static async ETTask OnRefreshClickAsync(EntityRef<DlgRoomList> selfRef)
        {
            DlgRoomList self = selfRef;
            if (self == null) { return; }
            await self.RefreshRoomList();
        }

        private static void OnCreateRoomClick(EntityRef<DlgRoomList> selfRef)
        {
            DlgRoomList self = selfRef;
            if (self == null) { return; }
            self.Root().GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_CreateRoom);
        }

        private static void OnBackClick(EntityRef<DlgRoomList> selfRef)
        {
            DlgRoomList self = selfRef;
            if (self == null) { return; }
            self.Root().GetComponent<UIComponent>().HideWindow(WindowID.WindowID_RoomList);
        }
    }
}
