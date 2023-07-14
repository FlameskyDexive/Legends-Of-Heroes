
using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    // [FriendOf(typeof(AccountComponent))]
    public static class DlgLoginSystem
    {

        public static void RegisterUIEvent(this DlgLogin self)
        {
            self.View.E_LoginButton.AddListener(self.OnLoginClickHandler);
        }

        public static void ShowWindow(this DlgLogin self, ShowWindowDataBase contextData = null)
        {
            
            self.View.E_LoginButton.interactable = true;
        }

        public static void OnLoginClickHandler(this DlgLogin self)
        {
            
            self.Login().Coroutine();

            self.View.E_LoginButton.interactable = false;
        }

        private static async ETTask Login(this DlgLogin self)
        {
            int res = await LoginHelper.Login(
                self.DomainScene(),
                self.View.E_AccountInputField.GetComponent<InputField>().text,
                self.View.E_PasswordInputField.GetComponent<InputField>().text);
            if (res == ErrorCode.ERR_Success)
            {
                self.DomainScene().GetComponent<NetworkCheckComponent>()?.StartCheck();
            }

        }


        public static void HideWindow(this DlgLogin self)
        {

        }

    }
}
