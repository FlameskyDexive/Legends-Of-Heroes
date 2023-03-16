using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
	[FriendOf(typeof(DlgRoom))]
	public static  class DlgRoomSystem
	{

		public static void RegisterUIEvent(this DlgRoom self)
        {
            self.View.E_ConfirmButton.AddListener(() =>
            {
                self.OnEnterMapClickHandler().Coroutine();
            });
            self.View.E_CancelButton.AddListener(() =>
            {
                self.OnCancelClickHandler().Coroutine();
            });

            self.View.ELoopScrollList_RolesLoopHorizontalScrollRect.AddItemRefreshListener((Transform transform, int index) =>
            {
                self.OnScrollItemRefreshHandler(transform, index);
            });

        }

		public static void ShowWindow(this DlgRoom self, ShowWindowDataBase contextData = null)
		{
            LobbyHelper.JoinOrCreateRoom(self.DomainScene()).Coroutine();

            self.RefreshRoomInfo();
        }

        public static void OnScrollItemRefreshHandler(this DlgRoom self, Transform transform, int index)
        {
            Scroll_Item_role itemRole = self.ScrollItemRoles[index].BindTrans(transform);
            PlayerInfoRoom playerInfo = self.ClientScene().GetComponent<RoomComponent>().PlayerInfos[index];
            //
            itemRole.E_RoleNameText.text = playerInfo.PlayerName;
            // itemRole.E_AvatarImage.sprite = ResComponent.Instance.LoadAsset<Sprite>($"Avatar{playerInfo.}");

        }
        
        // private static void RefreshRoleInfo(this DlgRoom, RoleInfo roleInfo, )


        public static async ETTask OnEnterMapClickHandler(this DlgRoom self)
        {
            await EnterMapHelper.EnterMapAsync(self.DomainScene());
            self.DomainScene().GetComponent<UIComponent>().HideWindow(WindowID.WindowID_Lobby);
        }

        public static async ETTask OnCancelClickHandler(this DlgRoom self)
        {
            self.DomainScene().GetComponent<UIComponent>().HideWindow(WindowID.WindowID_Room);
        }

        public static void RefreshRoomInfo(this DlgRoom self)
        {
            List<PlayerInfoRoom> playerInfos = self.ClientScene().GetComponent<RoomComponent>().PlayerInfos;
            if (playerInfos == null || playerInfos.Count == 0)
                return;
            int count = playerInfos.Count;
            self.RemoveUIScrollItems(ref self.ScrollItemRoles);
            self.AddUIScrollItems(ref self.ScrollItemRoles, count);
            self.View.ELoopScrollList_RolesLoopHorizontalScrollRect.SetVisible(true, count);
        }

    }
}
