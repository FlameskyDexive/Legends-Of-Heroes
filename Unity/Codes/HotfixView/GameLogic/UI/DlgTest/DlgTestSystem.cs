using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	public static  class DlgTestSystem
	{

		public static void RegisterUIEvent(this DlgTest self)
		{
			self.View.EButton_EnterMap.AddListener(()=> { self.OnEnterMapClickHandler(); });
			self.View.EButton_Test.AddListener(() => { self.OnTestClickHandler(); });
			self.View.ELoopScrollList_Test.AddItemRefreshListener((Transform Transform, int index) => {self.OnLoopListItemRefreshHandler(Transform,index);});
		}

		public static void ShowWindow(this DlgTest self, Entity contextData = null)
		{
			self.View.EText_Test.text = "test label";
			self.View.ESCommonUI.SetLabelContent("测试界面");

			int count = 100;
			self.AddUIScrollItems(ref self.ScrollItemServerTestsDict,count);
			
			self.View.ELoopScrollList_Test.SetVisible(true,count);
			
		}


		public static void HideWindow(this DlgTest self)
		{
			self.RemoveUIScrollItems(ref self.ScrollItemServerTestsDict);
		}

		
		public static void OnEnterMapClickHandler(this DlgTest self)
		{
			Log.Debug("Enter map");
			
			UIComponent.Instance.CloseWindow(WindowID.WindowID_Test);
			UIComponent.Instance.ShowWindow(WindowID.WindowID_Login);
			
		}

		public static void OnTestClickHandler(this DlgTest self)
		{
			Log.Debug("click Test");
		}


		public static void OnLoopListItemRefreshHandler(this DlgTest self, Transform transform, int index)
		{
			Scroll_Item_serverTest itemServerTest = self.ScrollItemServerTestsDict[index].BindTrans(transform);

			itemServerTest.ELabel_serverTest.text = $"{index}服";
			
		}


	}
}
