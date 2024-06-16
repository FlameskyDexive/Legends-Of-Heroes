using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
	[FriendOf(typeof(DlgLSLogin))]
	public static  class DlgLSLoginSystem
	{

		public static void RegisterUIEvent(this DlgLSLogin self)
		{
			self.View.ELoginBtnButton.AddListener(self.Root(),self.OnLogin);
					
		}

		public static void ShowWindow(this DlgLSLogin self, Entity contextData = null)
		{
		}

		 
		public static void OnLogin(this DlgLSLogin self)
		{
			LoginHelper.Login(
				self.Root(),
				self.View.EAccountInputField.text,
				self.View.EPasswordInputField.text).Coroutine();
		}
	}
}
