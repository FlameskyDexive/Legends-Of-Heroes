namespace ET.Client
{
    [FriendOf(typeof(DlgLSLogin))]
    public static class DlgLSLoginSystem
    {
        public static void RegisterUIEvent(this DlgLSLogin self)
        {
            EntityRef<DlgLSLogin> selfRef = self;
            self.View.ELoginBtnButton.AddListener(self.Root(), () => OnLoginButtonClicked(selfRef));
        }

        public static void ShowWindow(this DlgLSLogin self, Entity contextData = null)
        {
        }

        public static async ETTask OnLogin(this DlgLSLogin self)
        {
            string account = self.View.EAccountInputField != null ? self.View.EAccountInputField.text : string.Empty;
            string password = self.View.EPasswordInputField != null ? self.View.EPasswordInputField.text : string.Empty;
            GlobalComponent globalComponent = self.Root().GetComponent<GlobalComponent>();
            await LoginHelper.Login(self.Root(), globalComponent.GlobalConfig.Address, account, password);
        }

        private static void OnLoginButtonClicked(EntityRef<DlgLSLogin> selfRef)
        {
            DlgLSLogin dlg = selfRef;
            if (dlg == null)
            {
                return;
            }
            dlg.OnLogin().Coroutine();
        }
    }
}
