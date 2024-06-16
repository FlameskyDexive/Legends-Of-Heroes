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
            self.View.E_ConfirmButton.AddListener(self.Root(), () =>
            {
                // self.OnEnterMapClickHandler().Coroutine();
            });
            self.View.E_CancelButton.AddListener(self.Root(), () =>
            {
                self.OnCancelClickHandler().Coroutine();
            });

            self.View.ELoopScrollList_RolesLoopHorizontalScrollRect.AddItemRefreshListener((Transform transform, int index) =>
            {
                self.OnScrollItemRefreshHandler(transform, index);
            });

        }

		public static void ShowWindow(this DlgRoom self, Entity contextData = null)
		{
		}


        public static void OnScrollItemRefreshHandler(this DlgRoom self, Transform transform, int index)
        {
            Scroll_Item_role itemRole = self.ScrollItemRoles[index].BindTrans(transform);
            if (self.RoomInfo?.PlayerInfo?.Count > index)
            {
                PlayerInfo playerInfo = self.RoomInfo.PlayerInfo[index];
                itemRole.E_RoleNameText.text = playerInfo.PlayerName;
                itemRole.E_AvatarImage.sprite = self.Root().GetComponent<ResourcesLoaderComponent>().LoadAssetSync<Sprite>($"Avatar{playerInfo.AvatarIndex}");
            }

        }

        private static void Update(this DlgRoom self)
        {

        }
        

        public static async ETTask OnCancelClickHandler(this DlgRoom self)
        {
            self.Root().GetComponent<UIComponent>().HideWindow(WindowID.WindowID_Room);
        }

        public static void RefreshRoomInfo(this DlgRoom self, RoomInfo roomInfo)
        {
            self.RoomInfo = roomInfo;
            int count = roomInfo.PlayerInfo.Count;
            self.RemoveUIScrollItems(ref self.ScrollItemRoles);
            self.AddUIScrollItems(ref self.ScrollItemRoles, count);
            self.View.ELoopScrollList_RolesLoopHorizontalScrollRect.SetVisible(true, count);
            
            //判断是否准备好（房间满人），UI倒计时5s，随后跳转进入战斗。
            if (roomInfo.IsReady)
                self.StartCountDown().Coroutine();
        }

        public static async ETTask StartCountDown(this DlgRoom self)
        {
            //临时方便写法，正式写法换成开一个计时器或者Update里头更新倒计时。
            for (int i = 5; i > 0; i--)
            {
                self.View.ECountDownText.text = i.ToString();
                await self.Root().GetComponent<TimerComponent>().WaitAsync(1000);
            }
            await self.Root().GetComponent<TimerComponent>().WaitAsync(500);
            
        }



    }
}
