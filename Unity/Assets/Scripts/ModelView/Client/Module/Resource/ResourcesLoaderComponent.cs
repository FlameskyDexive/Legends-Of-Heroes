using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using YooAsset;
using SceneHandle = YooAsset.SceneHandle;

namespace ET.Client
{
    [EntitySystemOf(typeof(ResourcesLoaderComponent))]
    [FriendOf(typeof(ResourcesLoaderComponent))]
    public static partial class ResourcesLoaderComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ResourcesLoaderComponent self)
        {
            self.package = YooAssets.GetPackage("DefaultPackage");
        }
        
        [EntitySystem]
        private static void Awake(this ResourcesLoaderComponent self, string packageName)
        {
            self.package = YooAssets.GetPackage(packageName);
        }
        
        [EntitySystem]
        private static void Destroy(this ResourcesLoaderComponent self)
        {
            foreach (var kv in self.handlers)
            {
                self.ReleaseHandler(kv.Value);
            }
            self.ForceUnloadAllAssets();

            self.PackageVersion = string.Empty;
            self.Downloader = null;
            self.AssetsOperationHandles.Clear();
            self.SubAssetsOperationHandles.Clear();
            self.SceneOperationHandles.Clear();
            self.RawFileOperationHandles.Clear();
            self.HandleProgresses.Clear();
            self.DoneHandleQueue.Clear();
        }

        public static void ForceUnloadAllAssets(this ResourcesLoaderComponent self)
        {
            ResourcePackage package = YooAssets.GetPackage("DefaultPackage");
            package.UnloadAllAssetsAsync();
        }


        #region 热更相关

        public static async ETTask<int> UpdateVersionAsync(this ResourcesLoaderComponent self, int timeout = 30)
        {
            var package = YooAssets.GetPackage("DefaultPackage");
            var operation = package.RequestPackageVersionAsync(new RequestPackageVersionOptions(true, timeout));

            await operation;

            if (operation.Status != EOperationStatus.Succeeded)
            {
                return ErrorCode.ERR_ResourceUpdateVersionError;
            }

            self.PackageVersion = operation.PackageVersion;
            return ErrorCode.ERR_Success;
        }

        public static async ETTask<int> UpdateManifestAsync(this ResourcesLoaderComponent self)
        {
            var package = YooAssets.GetPackage("DefaultPackage");
            var operation = package.LoadPackageManifestAsync(new LoadPackageManifestOptions(self.PackageVersion, 60));

            await operation;

            if (operation.Status != EOperationStatus.Succeeded)
            {
                return ErrorCode.ERR_ResourceUpdateManifestError;
            }

            return ErrorCode.ERR_Success;
        }

        public static int CreateDownloader(this ResourcesLoaderComponent self)
        {
            int downloadingMaxNum = 10;
            int failedTryAgain = 3;
            ResourceDownloaderOperation downloader = self.package.CreateResourceDownloader(new ResourceDownloaderOptions(downloadingMaxNum, failedTryAgain));
            if (downloader.TotalDownloadCount == 0)
            {
                Log.Info("没有发现需要下载的资源");
            }
            else
            {
                Log.Info("一共发现了{0}个资源需要更新下载。".Fmt(downloader.TotalDownloadCount));
                self.Downloader = downloader;
            }

            return ErrorCode.ERR_Success;
        }

        public static async ETTask<int> DonwloadWebFilesAsync(this ResourcesLoaderComponent self,
        Action<DownloadFileStartedEventArgs> onStartDownloadFileCallback = null,
        Action<DownloadProgressChangedEventArgs> onDownloadProgress = null,
        Action<DownloadErrorEventArgs> onDownloadError = null,
        Action<DownloadCompletedEventArgs> onDownloadOver = null)
        {
            if (self.Downloader == null)
            {
                return ErrorCode.ERR_Success;
            }

            // 注册下载回调
            if (onStartDownloadFileCallback != null)
            {
                self.Downloader.DownloadFileStarted += onStartDownloadFileCallback;
            }
            if (onDownloadProgress != null)
            {
                self.Downloader.DownloadProgressChanged += onDownloadProgress;
            }
            if (onDownloadError != null)
            {
                self.Downloader.DownloadError += onDownloadError;
            }
            if (onDownloadOver != null)
            {
                self.Downloader.DownloadCompleted += onDownloadOver;
            }
            self.Downloader.StartDownload();
            await self.Downloader;

            // 检测下载结果
            if (self.Downloader.Status != EOperationStatus.Succeeded)
            {
                return ErrorCode.ERR_ResourceUpdateDownloadError;
            }

            return ErrorCode.ERR_Success;
        }

        #endregion


        public static void ReleaseHandler(this ResourcesLoaderComponent self,HandleBase handleBase)
        {
            switch (handleBase)
            {
                case AssetHandle handle:
                    handle.Release();
                    break;
                case AllAssetsHandle handle:
                    handle.Release();
                    break;
                case SubAssetsHandle handle:
                    handle.Release();
                    break;
                case RawFileHandle handle:
                    handle.Release();
                    break;
                case SceneHandle handle:
                    handle.UnloadSceneAsync();
                    break;
            }
        }
        
        public static  void UnLoadAssetSync(this ResourcesLoaderComponent self, string location) 
        {
            HandleBase handler;
            if (self.handlers.TryGetValue(location, out handler))
            {
                self.ReleaseHandler(handler);
                self.handlers.Remove(location);
            }
        }

        
        public static  T LoadAssetSync<T>(this ResourcesLoaderComponent self, string location) where T: UnityEngine.Object
        {
            HandleBase handler;
            if (!self.handlers.TryGetValue(location, out handler))
            {
                handler = self.package.LoadAssetSync<T>(location);
                
                self.handlers.Add(location, handler);
            }
            return (T)((AssetHandle)handler).AssetObject;
        }
        
        public static async ETTask<T> LoadAssetAsync<T>(this ResourcesLoaderComponent self, string location) where T : UnityEngine.Object
        {
            using CoroutineLock coroutineLock = await self.Root().GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.ResourcesLoader, location.GetHashCode());

            HandleBase handler;
            if (!self.handlers.TryGetValue(location, out handler))
            {
                handler = self.package.LoadAssetAsync<T>(location);

                await handler;

                self.handlers.Add(location, handler);
            }

            return (T)((AssetHandle)handler).AssetObject;
        }

        public static async ETTask<Dictionary<string, T>> LoadAllAssetsAsync<T>(this ResourcesLoaderComponent self, string location) where T : UnityEngine.Object
        {
            using CoroutineLock coroutineLock = await self.Root().GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.ResourcesLoader, location.GetHashCode());

            HandleBase handler;
            if (!self.handlers.TryGetValue(location, out handler))
            {
                handler = self.package.LoadAllAssetsAsync<T>(location);
                await handler;
                self.handlers.Add(location, handler);
            }

            Dictionary<string, T> dictionary = new Dictionary<string, T>();
            foreach (UnityEngine.Object assetObj in ((AllAssetsHandle)handler).AllAssetObjects)
            {
                T t = assetObj as T;
                dictionary.Add(t.name, t);
            }

            return dictionary;
        }

        public static async ETTask LoadSceneAsync(this ResourcesLoaderComponent self, string location, LoadSceneMode loadSceneMode)
        {
            using CoroutineLock coroutineLock = await self.Root().GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.ResourcesLoader, location.GetHashCode());

            HandleBase handler;
            if (self.handlers.TryGetValue(location, out handler))
            {
                return;
            }

            handler = self.package.LoadSceneAsync(location);

            await handler;
            self.handlers.Add(location, handler);
        }
    }
    
    /// <summary>
    /// 用来管理资源，生命周期跟随Parent，比如CurrentScene用到的资源应该用CurrentScene的ResourcesLoaderComponent来加载
    /// 这样CurrentScene释放后，它用到的所有资源都释放了
    /// </summary>
    [ComponentOf]
    public class ResourcesLoaderComponent: Entity, IAwake, IAwake<string>, IDestroy
    {
        public ResourcePackage package;
        public Dictionary<string, HandleBase> handlers = new();

        
        public string PackageVersion { get; set; }

        public ResourceDownloaderOperation Downloader { get; set; }

        public Dictionary<string, AssetHandle> AssetsOperationHandles = new Dictionary<string, AssetHandle>(100);

        public Dictionary<string, SubAssetsHandle> SubAssetsOperationHandles = new Dictionary<string, SubAssetsHandle>();

        public Dictionary<string, SceneHandle> SceneOperationHandles = new Dictionary<string, SceneHandle>();

        public Dictionary<string, RawFileHandle> RawFileOperationHandles = new Dictionary<string, RawFileHandle>(100);

        public Dictionary<HandleBase, Action<float>> HandleProgresses = new Dictionary<HandleBase, Action<float>>();

        public Queue<HandleBase> DoneHandleQueue = new Queue<HandleBase>();
    }
}
