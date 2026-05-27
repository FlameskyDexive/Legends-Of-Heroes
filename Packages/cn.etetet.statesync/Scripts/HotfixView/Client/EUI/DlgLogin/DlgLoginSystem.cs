namespace ET.Client
{
    [FriendOf(typeof(DlgLogin))]
    public static class DlgLoginSystem
    {
        public static void RegisterUIEvent(this DlgLogin self)
        {
            EntityRef<DlgLogin> selfRef = self;
            self.View.ELoginButton.AddListener(self.Root(), () => OnLoginButtonClicked(selfRef));
        }

        public static void ShowWindow(this DlgLogin self, Entity contextData = null)
        {
        }

        public static async ETTask OnLogin(this DlgLogin self)
        {
            string account = self.View.EAccountInputField != null ? self.View.EAccountInputField.text : string.Empty;
            string password = self.View.EPasswordInputField != null ? self.View.EPasswordInputField.text : string.Empty;
            GlobalComponent globalComponent = self.Root().GetComponent<GlobalComponent>();
            await LoginHelper.Login(self.Root(), globalComponent.GlobalConfig.Address, account, password);
        }

        private static void OnLoginButtonClicked(EntityRef<DlgLogin> selfRef)
        {
            DlgLogin dlg = selfRef;
            if (dlg == null)
            {
                return;
            }
            dlg.OnLogin().Coroutine();
        }
    }
}
