using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	[FriendClass(typeof(DlgRedDot))]
	public static  class DlgRedDotSystem
	{

		public static void RegisterUIEvent(this DlgRedDot self)
		{

			
		}

		public static void ShowWindow(this DlgRedDot self, Entity contextData = null)
		{
			
		}

		public static void OnBagNode1ClickHandler(this DlgRedDot self)
		{
			self.RedDotBagCount1 += 1;

		}
		 
		public static void OnBagNode2ClickHandler(this DlgRedDot self)
		{
			self.RedDotBagCount2 += 1;

		}
		
		
		public static void OnMailNode1ClickHandler(this DlgRedDot self)
		{
			self.RedDotMailCount1 += 1;
	
		}
		 
		public static void OnMailNode2ClickHandler(this DlgRedDot self)
		{
			self.RedDotMailCount2 += 1;

		}
		
	}
}
