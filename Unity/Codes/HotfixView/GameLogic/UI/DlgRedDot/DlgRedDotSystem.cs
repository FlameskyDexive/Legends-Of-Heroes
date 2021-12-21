using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	public static  class DlgRedDotSystem
	{

		public static void RegisterUIEvent(this DlgRedDot self)
		{
			self.View.EButton_BagNode1.AddListener(() => { self.OnBagNode1ClickHandler(); });
			self.View.EButton_BagNode2.AddListener(() => { self.OnBagNode2ClickHandler(); });
			self.View.EButton_MailNode1.AddListener(() => { self.OnMailNode1ClickHandler(); });
			self.View.EButton_MailNode2.AddListener(() => { self.OnMailNode2ClickHandler(); });
			
		}

		public static void ShowWindow(this DlgRedDot self, Entity contextData = null)
		{
			//生成根节点逻辑层
			RedDotHelper.AddRedDotNode(self.ZoneScene(),"Root",self.View.EButton_root.name,true);
			
			//生成背包和邮箱节点逻辑层
			RedDotHelper.AddRedDotNode(self.ZoneScene(),self.View.EButton_root.name,self.View.EButton_Bag.name,true);
			RedDotHelper.AddRedDotNode(self.ZoneScene(),self.View.EButton_root.name,self.View.EButton_Mail.name,true);
			
			//生成背包子节点 逻辑层
			RedDotHelper.AddRedDotNode(self.ZoneScene(),self.View.EButton_Bag.name,self.View.EButton_BagNode1.name,true);
			RedDotHelper.AddRedDotNode(self.ZoneScene(),self.View.EButton_Bag.name,self.View.EButton_BagNode2.name,true);	
			
			//生成邮箱子节点 逻辑层
			RedDotHelper.AddRedDotNode(self.ZoneScene(),self.View.EButton_Mail.name,self.View.EButton_MailNode1.name,true);
			RedDotHelper.AddRedDotNode(self.ZoneScene(),self.View.EButton_Mail.name,self.View.EButton_MailNode2.name,false);

			
			//为根节点添加显示层
			RedDotMonoView redDotMonoView = self.View.EButton_root.GetComponent<RedDotMonoView>();
			RedDotHelper.AddRedDotNodeView(self.ZoneScene(),self.View.EButton_root.name, redDotMonoView);
			
			//为背包功能分支添加显示层
			RedDotHelper.AddRedDotNodeView(self.ZoneScene(),self.View.EButton_Bag.name, self.View.EButton_Bag.gameObject,Vector3.one,Vector3.zero);
			RedDotHelper.AddRedDotNodeView(self.ZoneScene(),self.View.EButton_BagNode1.name, self.View.EButton_BagNode1.gameObject,Vector3.one,Vector3.zero);
			RedDotHelper.AddRedDotNodeView(self.ZoneScene(),self.View.EButton_BagNode2.name, self.View.EButton_BagNode2.gameObject,Vector3.one,Vector3.zero);
			
			//为邮箱功能分支添加显示层
			RedDotHelper.AddRedDotNodeView(self.ZoneScene(),self.View.EButton_Mail.name, self.View.EButton_Mail.gameObject,Vector3.one,Vector3.zero);
			RedDotHelper.AddRedDotNodeView(self.ZoneScene(),self.View.EButton_MailNode1.name, self.View.EButton_MailNode1.gameObject,Vector3.one,Vector3.zero);
			RedDotHelper.AddRedDotNodeView(self.ZoneScene(),self.View.EButton_MailNode2.name, self.View.EButton_MailNode2.gameObject,Vector3.one,Vector3.zero);
			
			//正式显示所有叶子节点
			RedDotHelper.ShowRedDotNode(self.ZoneScene(), self.View.EButton_BagNode1.name);
			RedDotHelper.ShowRedDotNode(self.ZoneScene(), self.View.EButton_BagNode2.name);
			RedDotHelper.ShowRedDotNode(self.ZoneScene(), self.View.EButton_MailNode1.name);
			RedDotHelper.ShowRedDotNode(self.ZoneScene(), self.View.EButton_MailNode2.name);
			
			
			//只允许刷新叶子节点的红点数量
			RedDotHelper.RefreshRedDotViewCount(self.ZoneScene(), self.View.EButton_BagNode1.name,self.RedDotBagCount1);
			RedDotHelper.RefreshRedDotViewCount(self.ZoneScene(), self.View.EButton_BagNode2.name,self.RedDotBagCount2);
			RedDotHelper.RefreshRedDotViewCount(self.ZoneScene(), self.View.EButton_MailNode1.name,self.RedDotMailCount1);
			RedDotHelper.RefreshRedDotViewCount(self.ZoneScene(), self.View.EButton_MailNode2.name,self.RedDotMailCount2);
			
		}

		public static void OnBagNode1ClickHandler(this DlgRedDot self)
		{
			self.RedDotBagCount1 += 1;
			RedDotHelper.RefreshRedDotViewCount(self.ZoneScene(), self.View.EButton_BagNode1.name,self.RedDotBagCount1);
		}
		 
		public static void OnBagNode2ClickHandler(this DlgRedDot self)
		{
			self.RedDotBagCount2 += 1;
			RedDotHelper.RefreshRedDotViewCount(self.ZoneScene(), self.View.EButton_BagNode2.name,self.RedDotBagCount2);
		}
		
		
		public static void OnMailNode1ClickHandler(this DlgRedDot self)
		{
			self.RedDotMailCount1 += 1;
			RedDotHelper.RefreshRedDotViewCount(self.ZoneScene(), self.View.EButton_MailNode1.name,self.RedDotMailCount1);
		}
		 
		public static void OnMailNode2ClickHandler(this DlgRedDot self)
		{
			self.RedDotMailCount2 += 1;
			RedDotHelper.RefreshRedDotViewCount(self.ZoneScene(), self.View.EButton_MailNode2.name,self.RedDotMailCount2);
		}
		
	}
}
