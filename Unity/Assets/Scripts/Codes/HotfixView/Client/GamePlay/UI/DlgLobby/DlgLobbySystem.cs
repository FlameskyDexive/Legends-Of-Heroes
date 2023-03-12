using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
	public static  class DlgLobbySystem
	{

		public static void RegisterUIEvent(this DlgLobby self)
		{
		  self.View.E_EnterMapButton.AddListener(()=>
		  {
			  self.OnEnterMapClickHandler().Coroutine();
		  });
		  self.View.E_SoloButton.AddListener(()=>
		  {
			  self.OnEnterMapClickHandler().Coroutine();
		  });
          
		
		}

		public static void ShowWindow(this DlgLobby self, ShowWindowDataBase contextData = null)
        {
            PlayerComponent playerComponent = self.ClientScene().GetComponent<PlayerComponent>();
            if (playerComponent != null)
            {
                Player player = playerComponent.MyPlayer;
                if (player != null)
                {
                    self.View.E_PlayerNameText.text = player.PlayerName;
                    self.View.E_AvatarEUIImage.sprite = ResComponent.Instance.LoadAsset<Sprite>($"Avatar{player.AvatarIndex}");
                }
            }
        }
		
		public static async ETTask OnEnterMapClickHandler(this DlgLobby self)
        {
            //await EnterMapHelper.EnterMapAsync(self.DomainScene());
            //self.DomainScene().GetComponent<UIComponent>().HideWindow(WindowID.WindowID_Lobby);
            self.DomainScene().GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_Room);
			// await EnterMapHelper.EnterMapAsync(self.ClientScene());
		}
        
        /// <summary>
		/// 发起创建/加入房间请求
		/// </summary>
		/// <param name="self"></param>
		/// <returns></returns>
		public static async ETTask OnSoloClickHandler(this DlgLobby self)
        {
            
		}
	}
}
