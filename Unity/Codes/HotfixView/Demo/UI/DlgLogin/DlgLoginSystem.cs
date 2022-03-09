using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ET
{
	public static  class DlgLoginSystem
	{

		public static void RegisterUIEvent(this DlgLogin self)
		{
			self.View.E_LoginButton.AddListener(() => { self.OnLoginClickHandler();});
		}

		public static void ShowWindow(this DlgLogin self, Entity contextData = null)
		{
			
		}
		
		public static void OnLoginClickHandler(this DlgLogin self)
		{
			LoginHelper.Login(
				self.DomainScene(), 
				ConstValue.LoginAddress, 
				self.View.E_AccountInputField.GetComponent<InputField>().text, 
				self.View.E_PasswordInputField.GetComponent<InputField>().text).Coroutine();
		}
		
		public static void HideWindow(this DlgLogin self)
		{

		}
		
	}
}
