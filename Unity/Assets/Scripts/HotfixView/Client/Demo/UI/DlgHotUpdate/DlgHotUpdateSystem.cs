using System.Collections;
using System.Collections.Generic;
using System;
using ET.EventType;
using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [Event(SceneType.Client)]
    public class OnPatchDownloadProgressEvent : AEvent<Scene, EventType.OnPatchDownloadProgress>
    {
        protected override async ETTask Run(Scene scene, OnPatchDownloadProgress a)
        {

            scene.GetComponent<UIComponent>()?.GetDlgLogic<DlgHotUpdate>()?.
                    OnPatchDownloadProgress(a.TotalDownloadCount, a.CurrentDownloadCount, a.TotalDownloadSizeBytes, a.CurrentDownloadSizeBytes);
            await ETTask.CompletedTask;
        }

    }

    [Event(SceneType.Client)]
    public class OnPatchDownlodFailedEvent : AEvent<Scene, OnPatchDownlodFailed>
    {
        protected override async ETTask Run(Scene scene, OnPatchDownlodFailed a)
        {
            Log.Error($"下载资源失败: {a.FileName} {a.Error}");
            await ETTask.CompletedTask;
        }
    }

    [FriendOf(typeof(DlgHotUpdate))]
	public static  class DlgHotUpdateSystem
	{

		public static void RegisterUIEvent(this DlgHotUpdate self)
		{
		 
		}

		public static void ShowWindow(this DlgHotUpdate self, ShowWindowDataBase contextData = null)
		{
		}


        public static void OnPatchDownloadProgress(this DlgHotUpdate self, int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes)
        {
            self.View.E_TxtLoadingText.text = $"{totalDownloadBytes}/{currentDownloadBytes}";
            self.View.E_ImgLoadingImage.fillAmount = 100.0f * currentDownloadBytes / totalDownloadBytes;
        }

    }
}
