using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
	[FriendOf(typeof(DlgLobby))]
	public static  class DlgLobbySystem
	{

		public static void RegisterUIEvent(this DlgLobby self)
		{
			self.View.EEnterMapButton.AddListenerAsync(self.Root(), self.EnterMap);
			self.View.EMatchButton.AddListenerAsync(self.Root(), self.Match);
			self.View.ECreateRoomButton.AddListener(self.Root(), self.OnCreateRoomClick);
			self.View.ERoomListButton.AddListener(self.Root(), self.OnRoomListClick);
		}

		public static void ShowWindow(this DlgLobby self, Entity contextData = null)
		{
		}

		public static async ETTask EnterMap(this DlgLobby self)
		{
			Scene root = self.Root();
			await EnterMapHelper.EnterMapAsync(root);
			root.GetComponent<UIComponent>().CloseWindow(WindowID.WindowID_Lobby);
		}


		public static async ETTask Match(this DlgLobby self)
		{
			Scene root = self.Root();
			await EnterMapHelper.StateSyncMatch(self.Fiber());
			root.GetComponent<UIComponent>().CloseWindow(WindowID.WindowID_Lobby);
		}

		public static void OnCreateRoomClick(this DlgLobby self)
		{
			self.Root().GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_CreateRoom);
		}

		public static void OnRoomListClick(this DlgLobby self)
		{
			self.Root().GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_RoomList);
		}

	}
}
