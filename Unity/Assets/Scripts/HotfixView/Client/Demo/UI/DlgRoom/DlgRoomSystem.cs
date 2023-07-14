using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
	[FriendOf(typeof(DlgRoom))]
	// [FriendOf(typeof(RoomComponent))]
	public static  class DlgRoomSystem
    {

        public class DlgRoomAwakeSystem : AwakeSystem<DlgRoom>
        {
            protected override void Awake(DlgRoom self)
            {
                // self.Awake();
            }
        }
        public class DlgRoomUpdateSystem : UpdateSystem<DlgRoom>
        {
            protected override void Update(DlgRoom self)
            {
                // if (Time.frameCount % 3 == 0)
                // self.Tick();

                self.Update();
            }
        }

        public class DlgRoomDestroySystem : DestroySystem<DlgRoom>
        {
            protected override void Destroy(DlgRoom self)
            {

            }
        }

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
            itemRole.E_AvatarImage.sprite = ResComponent.Instance.LoadAsset<Sprite>($"Avatar{playerInfo.AvatarIndex}");

        }

        private static void Update(this DlgRoom self)
        {
            
        }


        public static async ETTask OnEnterMapClickHandler(this DlgRoom self)
        {
            await EnterMapHelper.EnterMapAsync(self.Root());
            self.Root().GetComponent<UIComponent>().HideWindow(WindowID.WindowID_Lobby);
        }

        public static async ETTask OnCancelClickHandler(this DlgRoom self)
        {
            self.Root().GetComponent<UIComponent>().HideWindow(WindowID.WindowID_Room);
        }

        public static void RefreshRoomInfo(this DlgRoom self)
        {
            /*RoomComponent roomComponent = self.ClientScene().GetComponent<RoomComponent>();
            List<PlayerInfoRoom> playerInfos = roomComponent.PlayerInfos;
            if (playerInfos == null || playerInfos.Count == 0)
                return;
            int count = playerInfos.Count;
            self.RemoveUIScrollItems(ref self.ScrollItemRoles);
            self.AddUIScrollItems(ref self.ScrollItemRoles, count);
            self.View.ELoopScrollList_RolesLoopHorizontalScrollRect.SetVisible(true, count);
            //判断是否准备好（房间满人），UI倒计时5s，随后跳转进入战斗。
            if (roomComponent.RoomInfo.IsReady)
            {
                self.StartCountDown().Coroutine();
            }*/
        }

        public static async ETTask StartCountDown(this DlgRoom self)
        {
            //临时方便写法，正式写法换成开一个计时器或者Update里头更新倒计时。
            for (int i = 5; i > 0; i--)
            {
                self.View.ECountDownText.text = i.ToString();
                await TimerComponent.Instance.WaitAsync(1000);
            }
            await TimerComponent.Instance.WaitAsync(500);
            self.Root().GetComponent<UIComponent>().HideWindow(WindowID.WindowID_Room);
            self.Root().GetComponent<UIComponent>().HideWindow(WindowID.WindowID_Lobby);
        }

    }
}
