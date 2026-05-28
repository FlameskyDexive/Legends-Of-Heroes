using YooAsset;

namespace ET.Client
{
    // 热更补丁下载编排。版本号与 manifest 已在 ResourcesComponent.CreatePackageAsync 初始化时完成，
    // 这里只负责 HostPlayMode 下的增量补丁下载 + 进度 UI。编辑器模拟/离线模式下无补丁，直接返回。
    public static class HotUpdateHelper
    {
        public static async ETTask HotUpdateAsync(Scene root)
        {
            ResourcePackage package = YooAssets.GetPackage("DefaultPackage");
            if (package == null)
            {
                return;
            }

            ResourceDownloaderOperation downloader = package.CreateResourceDownloader(new ResourceDownloaderOptions(10, 3));
            if (downloader == null || downloader.TotalDownloadCount == 0)
            {
                // 没有需要下载的补丁
                return;
            }

            EntityRef<Scene> rootRef = root;

            // 打开热更界面
            await root.GetComponent<UIComponent>().ShowWindowAsync(WindowID.WindowID_HotUpdate);

            downloader.DownloadProgressChanged += data =>
            {
                Scene r = rootRef;
                EventSystem.Instance.Publish(r, new OnPatchDownloadProgress()
                {
                    TotalDownloadCount = data.TotalDownloadCount,
                    CurrentDownloadCount = data.CurrentDownloadCount,
                    TotalDownloadSizeBytes = data.TotalDownloadBytes,
                    CurrentDownloadSizeBytes = data.CurrentDownloadBytes,
                });
            };
            downloader.DownloadError += data =>
            {
                Scene r = rootRef;
                EventSystem.Instance.Publish(r, new OnPatchDownlodFailed() { FileName = data.FileName, Error = data.ErrorInfo });
            };

            downloader.StartDownload();
            await downloader;

            root = rootRef;
            if (downloader.Status != EOperationStatus.Succeeded)
            {
                Log.Error($"hot update patch download failed: {downloader.Error}");
            }

            root.GetComponent<UIComponent>().HideWindow(WindowID.WindowID_HotUpdate);
        }
    }
}
