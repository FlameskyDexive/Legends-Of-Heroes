using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	public static  class DlgLoginSystem
	{

		public static void RegisterUIEvent(this DlgLogin self)
		{
			self.View.EButton_LoginBtn.AddListener(self.OnLoginClickHandler);
		}

		public static void ShowWindow(this DlgLogin self, Entity contextData = null)
		{
			self.View.ESReuseUI.TestFunction("我在登录界面");
		}

		public static void OnLoginClickHandler(this DlgLogin self)
		{
			LoginHelper.Login(self.DomainScene(), ConstValue.LoginAddress, self.View.EInput_Account.text , self.View.EInput_Password.text).Coroutine();
		}

	}
}
