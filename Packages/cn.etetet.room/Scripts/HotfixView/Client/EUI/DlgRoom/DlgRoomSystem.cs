using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof(DlgRoom))]
    public static class DlgRoomSystem
    {
        public static void RegisterUIEvent(this DlgRoom self)
        {
            EntityRef<DlgRoom> selfRef = self;
            self.View.E_ConfirmButton.AddListener(self.Root(), () =>
            {
                // TODO: 接入 EnterMapHelper / 进入战斗流程后启用
            });
            self.View.E_CancelButton.AddListener(self.Root(), () => OnCancelClick(selfRef));
            self.View.ELoopScrollList_RolesLoopHorizontalScrollRect.AddItemRefreshListener((trans, index) => self.OnScrollItemRefreshHandler(trans, index));
        }

        public static void ShowWindow(this DlgRoom self, Entity contextData = null)
        {
        }

        public static void OnScrollItemRefreshHandler(this DlgRoom self, Transform transform, int index)
        {
            if (self.ScrollItemRoles == null || !self.ScrollItemRoles.ContainsKey(index))
            {
                return;
            }
            Scroll_Item_role itemRole = self.ScrollItemRoles[index].BindTrans(transform);
            if (self.RoomInfo?.PlayerInfo?.Count > index)
            {
                PlayerInfo playerInfo = self.RoomInfo.PlayerInfo[index];
                itemRole.E_RoleNameText.text = playerInfo.PlayerName;
                itemRole.E_AvatarImage.sprite = self.Root().GetComponent<ResourcesLoaderComponent>().LoadAssetSync<Sprite>($"Avatar{playerInfo.AvatarIndex}");
            }
        }

        public static void RefreshRoomInfo(this DlgRoom self, RoomInfo roomInfo)
        {
            self.RoomInfo = roomInfo;
            int count = roomInfo.PlayerInfo.Count;
            self.RemoveUIScrollItems(ref self.ScrollItemRoles);
            self.AddUIScrollItems(ref self.ScrollItemRoles, count);
            self.View.ELoopScrollList_RolesLoopHorizontalScrollRect.SetVisible(true, count);

            if (roomInfo.IsReady)
            {
                self.StartCountDown().Coroutine();
            }
        }

        [EnableGetComponent(typeof(TimerComponent))]
        public static async ETTask StartCountDown(this DlgRoom self)
        {
            EntityRef<DlgRoom> selfRef = self;
            for (int i = 5; i > 0; i--)
            {
                self = selfRef;
                if (self == null) { return; }
                self.View.ECountDownText.text = i.ToString();
                await self.Root().GetComponent<TimerComponent>().WaitAsync(1000);
            }
            self = selfRef;
            if (self == null) { return; }
            await self.Root().GetComponent<TimerComponent>().WaitAsync(500);
        }

        private static void OnCancelClick(EntityRef<DlgRoom> selfRef)
        {
            DlgRoom self = selfRef;
            if (self == null) { return; }
            self.Root().GetComponent<UIComponent>().HideWindow(WindowID.WindowID_Room);
        }
    }
}
