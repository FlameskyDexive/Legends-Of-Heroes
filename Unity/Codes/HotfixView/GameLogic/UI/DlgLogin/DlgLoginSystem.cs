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
			self.View.EButton_LoginBtn.AddListener(() => { self.OnLoginClickHandler();});
			//注册循环列表事件
			self.View.ELoopScrollList_Test.AddItemRefreshListener((transform,index)=> { self.OnRefreshItemHandler(transform,index); });
		}

		public static void ShowWindow(this DlgLogin self, Entity contextData = null)
		{
			//使用循环列表
			//int count = 100;
			//self.AddUIScrollItems(ref self.ItemsDictionary,count);
			//self.View.ELoopScrollList_Test.SetVisible(true,count);
			
			self.View.ESCommonUI.SetLabelContent("登录界面");
			
			
		}
		
		public static void OnLoginClickHandler(this DlgLogin self)
		{
			UIComponent.Instance.CloseWindow(WindowID.WindowID_Login);
			UIComponent.Instance.ShowWindow(WindowID.WindowID_Test);
			
			//LoginHelper.Login(self.DomainScene(), ConstValue.LoginAddress, self.View.EInput_Account.text , self.View.EInput_Password.text).Coroutine();
		}

		public static void OnRefreshItemHandler(this DlgLogin self,Transform transform,int index)
		{
			Scroll_Item_test test = self.ItemsDictionary[index].BindTrans(transform);

			test.ELabel_Content.text = $"第{index}服";
			test.EButton_Select.AddListener(()=>
			{
				int localIndex = index;
				self.OnScrollItemClickHandler(localIndex);
			});
		}
		
		public static void HideWindow(this DlgLogin self)
		{
			self.RemoveUIScrollItems(ref self.ItemsDictionary);
		}


		public static void OnScrollItemClickHandler(this DlgLogin self,int index)
		{
			
		}
		
	}
}
