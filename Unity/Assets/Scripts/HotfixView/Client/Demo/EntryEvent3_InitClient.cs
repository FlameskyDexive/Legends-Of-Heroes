using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ET.Client
{
    [Event(SceneType.Main)]
    public class EntryEvent3_InitClient: AEvent<Scene, EntryEvent3>
    {
        protected override async ETTask Run(Scene root, EntryEvent3 args)
        {
            GlobalComponent globalComponent = root.AddComponent<GlobalComponent>();
            root.AddComponent<UIPathComponent>();
            root.AddComponent<UIEventComponent>();
            root.AddComponent<UIComponent>();
            root.AddComponent<ResourcesLoaderComponent>();
            root.AddComponent<PlayerComponent>();
            root.AddComponent<CurrentScenesComponent>();
            await root.AddComponent<RedDotComponent>().PreLoadGameObject();
            
            // 根据配置修改掉Main Fiber的SceneType
            // SceneType sceneType = EnumHelper.FromString<SceneType>(globalComponent.GlobalConfig.AppType.ToString());
            SceneType sceneType = EnumHelper.FromString<SceneType>(GlobalConfig.Instance.AppType.ToString());
            root.SceneType = sceneType;
            SkillConfig skillConfig = SkillConfigCategory.Instance.Get(1001, 1);
            Log.Info($"{skillConfig?.Desc}");

            // 热更流程
            await HotUpdateAsync(root);
            // await EventSystem.Instance.PublishAsync(root, new AppStartInitFinish());
        }

        private static async ETTask HotUpdateAsync(Scene root)
        {

            // 打开热更界面
            await root.GetComponent<UIComponent>().ShowWindowAsync(WindowID.WindowID_HotUpdate);

            // 更新版本号
            int errorCode = await root.GetComponent<ResourcesLoaderComponent>().UpdateVersionAsync();
            if (errorCode != ErrorCode.ERR_Success)
            {
                Log.Error("FsmUpdateStaticVersion 出错！{0}".Fmt(errorCode));
                return;
            }

            // 更新Manifest
            errorCode = await root.GetComponent<ResourcesLoaderComponent>().UpdateManifestAsync();
            if (errorCode != ErrorCode.ERR_Success)
            {
                Log.Error("ResourceComponent.UpdateManifest 出错！{0}".Fmt(errorCode));
                return;
            }

            // 创建下载器
            errorCode = root.GetComponent<ResourcesLoaderComponent>().CreateDownloader();
            if (errorCode != ErrorCode.ERR_Success)
            {
                Log.Error("ResourceComponent.FsmCreateDownloader 出错！{0}".Fmt(errorCode));
                return;
            }

            // Downloader不为空，说明有需要下载的资源
            if (root.GetComponent<ResourcesLoaderComponent>().Downloader != null)
            {
                await DownloadPatch(root);
            }
            else
            {
                await EnterGame(root);
            }
        }

        private static async ETTask DownloadPatch(Scene root)
        {
            // 下载资源
            Log.Info("Count: {0}, Bytes: {1}".Fmt(root.GetComponent<ResourcesLoaderComponent>().Downloader.TotalDownloadCount, root.GetComponent<ResourcesLoaderComponent>().Downloader.TotalDownloadBytes));
            int errorCode = await root.GetComponent<ResourcesLoaderComponent>().DonwloadWebFilesAsync(
                // 开始下载回调
                null,

                // 下载进度回调
                (totalDownloadCount, currentDownloadCount, totalDownloadBytes, currentDownloadBytes) =>
                {
                    // 更新进度条
                    EventSystem.Instance.Publish(root, new OnPatchDownloadProgress() { TotalDownloadCount = totalDownloadCount, CurrentDownloadCount = currentDownloadCount, TotalDownloadSizeBytes = totalDownloadBytes, CurrentDownloadSizeBytes = currentDownloadBytes });
                },

                // 下载失败回调
                (fileName, error) =>
                {
                    // 下载失败
                    EventSystem.Instance.Publish(root, new OnPatchDownlodFailed() { FileName = fileName, Error = error });
                },

                // 下载完成回调
                null);

            if (errorCode != ErrorCode.ERR_Success)
            {
                // todo: 弹出错误提示，确定后重试。
                Log.Error("ResourceComponent.FsmDonwloadWebFiles 出错！{0}".Fmt(errorCode));
                return;
            }

            int codeVersion = GlobalConfig.Instance.CodeVersion;
            await ResourcesComponent.Instance.RestartAsync();
            bool codeChanged = codeVersion != GlobalConfig.Instance.CodeVersion;

            if (codeChanged)
            {
                // 如果dll文件有更新，则需要重启。
                GameObject.Find("Global").GetComponent<Init>().Restart().Coroutine();
            }
            else
            {
                await EnterGame(root);
            }
        }

        private static async ETTask EnterGame(Scene root)
        {
            root.GetComponent<UIComponent>().HideWindow(WindowID.WindowID_HotUpdate);
            root.GetComponent<UIComponent>().CloseAllWindow();
            await root.GetComponent<UIComponent>().ShowWindowAsync(WindowID.WindowID_Login);
            // 只是资源更新就直接进入游戏。
            await EventSystem.Instance.PublishAsync(root, new AppStartInitFinish());
        }
    }
}