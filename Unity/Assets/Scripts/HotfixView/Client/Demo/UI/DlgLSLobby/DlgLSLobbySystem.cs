using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
	[FriendOf(typeof(DlgLSLobby))]
	public static  class DlgLSLobbySystem
	{

		public static void RegisterUIEvent(this DlgLSLobby self)
		{
		   self.View.EMatchButton.AddListenerAsync(self.Root(), self.EnterMap);
		   self.View.EReplayButton.AddListener(self.Root(),self.Replay);
		}

		public static void ShowWindow(this DlgLSLobby self, Entity contextData = null)
		{
		}

		private static async ETTask EnterMap(this DlgLSLobby self)
		{
			await EnterMapHelper.Match(self.Fiber());
		}
        
		private static void Replay(this DlgLSLobby self)
		{
			byte[] bytes = File.ReadAllBytes(self.View.EReplayPathInputField.text);
            
			Replay replay = MemoryPackHelper.Deserialize(typeof (Replay), bytes, 0, bytes.Length) as Replay;
			Log.Debug($"start replay: {replay.Snapshots.Count} {replay.FrameInputs.Count} {replay.UnitInfos.Count}");
			LSSceneChangeHelper.SceneChangeToReplay(self.Root(), replay).Coroutine();
		}

	}
}
