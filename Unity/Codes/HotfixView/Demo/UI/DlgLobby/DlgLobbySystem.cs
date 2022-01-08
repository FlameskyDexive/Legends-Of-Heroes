using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	public static  class DlgLobbySystem
	{

		public static void RegisterUIEvent(this DlgLobby self)
		{
		  self.View.EButton_EnterMap.AddListener(()=>
		  {
			  self.OnEnterMapClickHandler().Coroutine();
		  });
		
		}

		public static void ShowWindow(this DlgLobby self, Entity contextData = null)
		{
			//调用公共UI
			self.View.ESReuseUI.TestFunction("进入游戏大厅");
			
			//使用无任何组件的 GameObject
			self.View.EGBackGround.GetComponent<RectTransform>();

		}
		
		public static async ETTask OnEnterMapClickHandler(this DlgLobby self)
		{
			await EnterMapHelper.EnterMapAsync(self.ZoneScene());
		}
	}
}
