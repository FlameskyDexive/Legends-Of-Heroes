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
		}

		public static void ShowWindow(this DlgTest self, Entity contextData = null)
		{
			self.View.EText_Test.text = "test label";
		}

		public static void OnEnterMapClickHandler(this DlgTest self)
		{
			Log.Debug("Enter map");
		}

		public static void OnTestClickHandler(this DlgTest self)
		{
			Log.Debug("click Test");
		}
		
	}
}
