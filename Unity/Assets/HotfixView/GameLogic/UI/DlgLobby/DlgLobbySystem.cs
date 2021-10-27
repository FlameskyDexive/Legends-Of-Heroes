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
		  self.View.ELoopScrollList_Test.AddItemRefreshListener(self.OnRefreshItemHandler);
		}

		public static void ShowWindow(this DlgLobby self, Entity contextData = null)
		{
			self.View.ESReuseUI.TestFunction("我在大厅");

			int count = 10;
			self.AddUIScrollItems(ref self.ItemsDictionary,count);

			self.View.ELoopScrollList_Test.totalCount = count;
			self.View.ELoopScrollList_Test.RefillCells();
			//self.View.ELoopScrollList_Test.SetVisible(true,count);
		}


		public static void HideWindow(this DlgLobby self)
		{
			self.RemoveUIScrollItems(ref self.ItemsDictionary);
		}

		
		public static void OnEnterMapClickHandler(this DlgLobby self)
		{
			EnterMapHelper.EnterMapAsync(self.ZoneScene()).Coroutine();
		}
		 
		
		public static void OnRefreshItemHandler(this DlgLobby self,Transform transform,int index)
		{
			Scroll_Item_test test = self.ItemsDictionary[index].BindTrans(transform);

			test.ELabel_Content.text = index.ToString();

		}

	}
}
