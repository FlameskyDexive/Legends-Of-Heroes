using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [Event(SceneType.Demo)]
    public class OnPatchDownloadProgressEvent : AEvent<Scene, OnPatchDownloadProgress>
    {
        protected override async ETTask Run(Scene scene, OnPatchDownloadProgress a)
        {

            scene.GetComponent<UIComponent>()?.GetDlgLogic<DlgHotUpdate>()?.
                    OnPatchDownloadProgress(a.TotalDownloadCount, a.CurrentDownloadCount, a.TotalDownloadSizeBytes, a.CurrentDownloadSizeBytes);
            await ETTask.CompletedTask;
        }

    }

    [Event(SceneType.Demo)]
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

		public static void ShowWindow(this DlgHotUpdate self, Entity contextData = null)
		{
		}
        
        public static void OnPatchDownloadProgress(this DlgHotUpdate self, int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes)
        {
            float progress = (float)currentDownloadBytes / totalDownloadBytes;
            self.View.E_ImgLoadingImage.fillAmount = progress;
            self.View.E_TxtLoadingText.text = $"{ByteHelper.FormatBytes(currentDownloadBytes)}/{ByteHelper.FormatBytes(totalDownloadBytes)}, {(progress * 100).ToString("f2")}%";
        }

    }
}
