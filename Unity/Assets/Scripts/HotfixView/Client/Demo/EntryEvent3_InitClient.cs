using System;
using System.IO;
using UnityEngine;
using YooAsset;

namespace ET.Client
{
    [Event(SceneType.Main)]
    public class EntryEvent3_InitClient: AEvent<Scene, EventType.EntryEvent3>
    {
        protected override async ETTask Run(Scene root, EventType.EntryEvent3 args)
        {
            World.Instance.AddSingleton<UIEventComponent>();
            
            root.AddComponent<TimerComponent>();
            root.AddComponent<CoroutineLockComponent>();
            
            GlobalComponent globalComponent = root.AddComponent<GlobalComponent>();
            root.AddComponent<UIGlobalComponent>();
            root.AddComponent<UIComponent>();
            // ResourcesComponent resourcesComponent = root.AddComponent<ResourcesComponent>();
            // root.AddComponent<ResourcesLoaderComponent>();
            root.AddComponent<PlayerComponent>();
            root.AddComponent<CurrentScenesComponent>();
            
            // await resourcesComponent.LoadBundleAsync("unit.unity3d");
            
            // 根据配置修改掉Main Fiber的SceneType
            SceneType sceneType = EnumHelper.FromString<SceneType>(GlobalConfig.Instance.AppType.ToString());
            root.SceneType = sceneType;
            
            // 热更流程
            await HotUpdateAsync(root);
            // await EventSystem.Instance.PublishAsync(root, new EventType.AppStartInitFinish());
        }
        
        private static async ETTask HotUpdateAsync(Scene clientScene)
        {

            // 打开热更界面
            // await fuiComponent.ShowPanelAsync(PanelId.HotUpdatePanel);
            await clientScene.GetComponent<UIComponent>().ShowWindowAsync(WindowID.WindowID_HotUpdate);

            // 更新版本号
            int errorCode = await ResComponent.Instance.UpdateVersionAsync();
            if (errorCode != ErrorCode.ERR_Success)
            {
                Log.Error("FsmUpdateStaticVersion 出错！{0}".Fmt(errorCode));
                return;
            }

            // 更新Manifest
            errorCode = await ResComponent.Instance.UpdateManifestAsync();
            if (errorCode != ErrorCode.ERR_Success)
            {
                Log.Error("ResourceComponent.UpdateManifest 出错！{0}".Fmt(errorCode));
                return;
            }

            // 创建下载器
            errorCode = ResComponent.Instance.CreateDownloader();
            if (errorCode != ErrorCode.ERR_Success)
            {
                Log.Error("ResourceComponent.FsmCreateDownloader 出错！{0}".Fmt(errorCode));
                return;
            }

            // Downloader不为空，说明有需要下载的资源
            if (ResComponent.Instance.Downloader != null)
            {
                await DownloadPatch(clientScene);
            }
            else
            {
                await EnterGame(clientScene);
            }
        }

        private static async ETTask DownloadPatch(Scene clientScene)
        {
            // 下载资源
            Log.Info("Count: {0}, Bytes: {1}".Fmt(ResComponent.Instance.Downloader.TotalDownloadCount, ResComponent.Instance.Downloader.TotalDownloadBytes));
            int errorCode = await ResComponent.Instance.DonwloadWebFilesAsync(
                // 开始下载回调
                null,

                // 下载进度回调
                (totalDownloadCount, currentDownloadCount, totalDownloadBytes, currentDownloadBytes) =>
                {
                    // 更新进度条
                    EventSystem.Instance.Publish(clientScene, new EventType.OnPatchDownloadProgress() { TotalDownloadCount = totalDownloadCount, CurrentDownloadCount = currentDownloadCount, TotalDownloadSizeBytes = totalDownloadBytes, CurrentDownloadSizeBytes = currentDownloadBytes });
                },

                // 下载失败回调
                (fileName, error) =>
                {
                    // 下载失败
                    EventSystem.Instance.Publish(clientScene, new EventType.OnPatchDownlodFailed() { FileName = fileName, Error = error });
                },

                // 下载完成回调
                null);

            if (errorCode != ErrorCode.ERR_Success)
            {
                // todo: 弹出错误提示，确定后重试。
                Log.Error("ResourceComponent.FsmDonwloadWebFiles 出错！{0}".Fmt(errorCode));
                return;
            }

            int modelVersion = GlobalConfig.Instance.ModelVersion;
            int hotFixVersion = GlobalConfig.Instance.HotFixVersion;
            await MonoResComponent.Instance.RestartAsync();
            bool modelChanged = modelVersion != GlobalConfig.Instance.ModelVersion;
            bool hotfixChanged = hotFixVersion != GlobalConfig.Instance.HotFixVersion;

            if (modelChanged || hotfixChanged)
            {
                // 如果dll文件有更新，则需要重启。
                GameObject.Find("Global").GetComponent<Init>().Restart();
            }
            else
            {
                await EnterGame(clientScene);
            }
        }

        private static async ETTask EnterGame(Scene clientScene)
        {
            clientScene.GetComponent<UIComponent>().HideWindow(WindowID.WindowID_HotUpdate);
            clientScene.GetComponent<UIComponent>().CloseAllWindow();
            await clientScene.GetComponent<UIComponent>().ShowWindowAsync(WindowID.WindowID_Login);
            // 只是资源更新就直接进入游戏。
            await EventSystem.Instance.PublishAsync(clientScene, new EventType.AppStartInitFinish());
        }
    }
}