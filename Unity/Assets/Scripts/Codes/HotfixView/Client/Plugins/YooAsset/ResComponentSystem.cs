using System;
using UnityEngine.SceneManagement;
using YooAsset;

namespace ET.Client
{
   public class ResComponentAwakeSystem: AwakeSystem<ResComponent>
    {
        protected override void Awake(ResComponent self)
        {
            self.Awake();
        }
    }
    
    public class ResComponentDestroySystem: DestroySystem<ResComponent>
    {
        protected override void Destroy(ResComponent self)
        {
            self.Destroy(); 
        }
    }
    
    public class ResComponentUpdateSystem: UpdateSystem<ResComponent>
    {
        protected override void Update(ResComponent self)
        { 
            self.Update();
        }
    }
    [FriendOf(typeof(ResComponent))]
    public static class ResComponentSystem
    {
        #region 生命周期

        public static void Awake(this ResComponent self)
        {
            ResComponent.Instance = self;
        }
        
        public static ETTask InitResourceAsync(this ResComponent self, Scene clientScene)
        {
            ETTask task = ETTask.Create(true); 
            FsmComponent fsmComponent = clientScene.AddComponent<FsmComponent, ETTask>(task);
            
            fsmComponent.AddNodeHandler(nameof(FsmResourceInit));
            fsmComponent.AddNodeHandler(nameof(FsmUpdateStaticVersion));
            fsmComponent.AddNodeHandler(nameof(FsmUpdateManifest));
            fsmComponent.AddNodeHandler(nameof(FsmCreateDownloader));
            fsmComponent.AddNodeHandler(nameof(FsmDonwloadWebFiles));
            fsmComponent.AddNodeHandler(nameof(FsmPatchDone));
            
            fsmComponent.Run(nameof(FsmResourceInit));

            return task;
        }

        public static void Destroy(this ResComponent self)
        {
            ResComponent.Instance = null;
            self.PackageVersion = string.Empty;
            self.Downloader = null;

            foreach (var handle in self.AssetsOperationHandles.Values)
            {
                handle.Release();
            }

            foreach (var handle in self.SubAssetsOperationHandles.Values)
            {
                handle.Release();
            }
        }

        public static void Update(this ResComponent self)
        {
            foreach (var kv in self.HandleProgresses)
            {
                OperationHandleBase handle = kv.Key;
                Action<float> progress = kv.Value;

                if (!handle.IsValid)
                {
                    continue;
                }

                if (handle.IsDone)
                {
                    self.DoneHandleQueue.Enqueue(handle);
                    progress?.Invoke(1);
                    continue;
                }

                progress?.Invoke(handle.Progress);
            }

            while (self.DoneHandleQueue.Count > 0)
            {
                var handle = self.DoneHandleQueue.Dequeue();
                self.HandleProgresses.Remove(handle);
            }
        }

        #endregion

        #region 热更相关

        public static async ETTask<int> UpdateVersionAsync(this ResComponent self, int timeout = 30)
        {
            var package = YooAssets.GetAssetsPackage("DefaultPackage");
            var operation = package.UpdatePackageVersionAsync();
            
            await operation.GetAwaiter();

            if (operation.Status != EOperationStatus.Succeed)
            {
                return ErrorCode.ERR_ResourceUpdateVersionError;
            }

            self.PackageVersion = operation.PackageVersion;
            return ErrorCode.ERR_Success;
        }

        public static async ETTask<int> UpdateManifestAsync(this ResComponent self)
        {
             var package = YooAssets.GetAssetsPackage("DefaultPackage");
            var operation = package.UpdatePackageManifestAsync(self.PackageVersion);
                        
            await operation.GetAwaiter();

            if (operation.Status != EOperationStatus.Succeed)
            {
                return ErrorCode.ERR_ResourceUpdateManifestError;
            }

            return ErrorCode.ERR_Success;
        }

        public static int CreateDownloader(this ResComponent self)
        {
            int downloadingMaxNum = 10;
            int failedTryAgain = 3;
            PatchDownloaderOperation downloader = YooAssets.CreatePatchDownloader(downloadingMaxNum, failedTryAgain);
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

        public static async ETTask<int> DonwloadWebFilesAsync(this ResComponent self, DownloaderOperation.OnDownloadProgress onDownloadProgress = null, DownloaderOperation.OnDownloadError onDownloadError = null)
        {
            if (self.Downloader == null)
            {
                return ErrorCode.ERR_Success;
            }

            // 注册下载回调
            self.Downloader.OnDownloadProgressCallback = onDownloadProgress;
            self.Downloader.OnDownloadErrorCallback = onDownloadError;
            self.Downloader.BeginDownload();
            await self.Downloader.GetAwaiter();

            // 检测下载结果
            if (self.Downloader.Status != EOperationStatus.Succeed)
            {
                return ErrorCode.ERR_ResourceUpdateDownloadError;
            }

            return ErrorCode.ERR_Success;
        }

        #endregion

        #region 卸载

        public static void UnloadUnusedAssets(this ResComponent self)
        {
            AssetsPackage package = YooAssets.GetAssetsPackage("DefaultPackage");
            package.UnloadUnusedAssets();
        }

        public static void ForceUnloadAllAssets(this ResComponent self)
        {
            AssetsPackage package = YooAssets.GetAssetsPackage("DefaultPackage");
            package.ForceUnloadAllAssets();
        }

        public static void UnloadAsset(this ResComponent self, string location)
        {
            self.AssetsOperationHandles.TryGetValue(location, out AssetOperationHandle handle);
            handle.Release();
        }

        #endregion

        #region 异步加载

        public static async ETTask<T> LoadAssetAsync<T>(this ResComponent self, string location) where T: UnityEngine.Object
        {
            self.AssetsOperationHandles.TryGetValue(location, out AssetOperationHandle handle);

            if (handle == null)
            {
                handle = YooAssets.LoadAssetAsync<T>(location);
                self.AssetsOperationHandles[location] = handle;
            }

            await handle;

            return handle.GetAssetObject<T>();
        }

        public static async ETTask<UnityEngine.Object> LoadAssetAsync(this ResComponent self, string location, Type type)
        {
            if (!self.AssetsOperationHandles.TryGetValue(location, out AssetOperationHandle handle))
            {
                handle = YooAssets.LoadAssetAsync(location, type);
                self.AssetsOperationHandles[location] = handle;
            }

            await handle;

            return handle.AssetObject;
        }

        public static async ETTask<UnityEngine.SceneManagement.Scene> LoadSceneAsync(this ResComponent self, string location, Action<float> progressCallback = null, LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100)
        {
            if (!self.SceneOperationHandles.TryGetValue(location, out SceneOperationHandle handle))
            {
                handle = YooAssets.LoadSceneAsync(location, loadSceneMode, activateOnLoad, priority);
                self.SceneOperationHandles[location] = handle;
            }

            if (progressCallback != null)
            {
                self.HandleProgresses.Add(handle, progressCallback);
            }

            await handle;

            return handle.SceneObject;
        }

        public static async ETTask<byte[]> LoadRawFileDataAsync(this ResComponent self, string location)
        {
            if (!self.RawFileOperationHandles.TryGetValue(location, out RawFileOperationHandle handle))
            {
                handle = YooAssets.LoadRawFileAsync(location);
                self.RawFileOperationHandles[location] = handle;
            }
            
            await handle;
            
            return handle.GetRawFileData();
        }

        public static async ETTask<string> LoadRawFileTextAsync(this ResComponent self, string location)
        {
            if (!self.RawFileOperationHandles.TryGetValue(location, out RawFileOperationHandle handle))
            {
                handle = YooAssets.LoadRawFileAsync(location);
                self.RawFileOperationHandles[location] = handle;
            }
            
            await handle;
            
            return handle.GetRawFileText();
        }

        #endregion

        #region 同步加载

        public static T LoadAsset<T>(this ResComponent self, string location)where T: UnityEngine.Object
        {
            self.AssetsOperationHandles.TryGetValue(location, out AssetOperationHandle handle);

            if (handle == null)
            {
                handle = YooAssets.LoadAssetSync<T>(location);
                self.AssetsOperationHandles[location] = handle;
            }

            return handle.AssetObject as T;
        }
        
        public static UnityEngine.Object LoadAsset(this ResComponent self, string location, Type type)
        {
            self.AssetsOperationHandles.TryGetValue(location, out AssetOperationHandle handle);

            if (handle == null)
            {
                handle = YooAssets.LoadAssetSync(location, type);
                self.AssetsOperationHandles[location] = handle;
            }

            return handle.AssetObject;
        }

        #endregion

    } 
}