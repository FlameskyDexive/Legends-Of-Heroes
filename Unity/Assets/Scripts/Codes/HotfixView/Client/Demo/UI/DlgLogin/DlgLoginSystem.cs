using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
	[FriendOf(typeof(DlgLogin))]
	public static  class DlgLoginSystem
	{

		public static void RegisterUIEvent(this DlgLogin self)
		{
		   self.View.E_LoginButton.onClick.AddListener(self.OnLoginClickHandler);
		}

		public static void ShowWindow(this DlgLogin self, Entity contextData = null)
		{

			
		}

		public static void OnLoginClickHandler(this DlgLogin self)
		{
			LoginHelper.Login(self.ClientScene(), self.View.E_AccountInputField.text, self.View.E_PasswordInputField.text).Coroutine();
		}
		
	}
}
