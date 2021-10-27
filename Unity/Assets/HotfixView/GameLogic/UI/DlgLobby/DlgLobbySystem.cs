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
		  self.View.EButton_EnterMap.AddListener(self.OnEnterMapClickHandler);
		
		}

		public static void ShowWindow(this DlgLobby self, Entity contextData = null)
		{
			self.View.ESReuseUI.TestFunction("进入游戏大厅");


		}
		
		public static void OnEnterMapClickHandler(this DlgLobby self)
		{
			EnterMapHelper.EnterMapAsync(self.ZoneScene()).Coroutine();
		}
	}
}
