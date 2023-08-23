using System;
using System.Collections;
using System.IO;
using UnityEngine;
using YooAsset;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ET
{
    public class MonoResComponent
    {
        public static MonoResComponent Instance { get; private set; } = new MonoResComponent();

        private ResourcePackage defaultPackage;
        private EPlayMode playMode;

        public async ETTask InitAsync(EPlayMode mode)
        {
            this.playMode = mode;
            // 初始化资源系统
            YooAssets.Initialize();
            YooAssets.SetOperationSystemMaxTimeSlice(30);

            await InitPackage();

            await this.LoadGlobalConfigAsync();
        }

        public async ETTask RestartAsync()
        {
            await this.LoadGlobalConfigAsync();
        }

        private async ETTask InitPackage()
        {

            // 创建默认的资源包
            string packageName = "DefaultPackage";
            defaultPackage = YooAssets.TryGetPackage(packageName);
            if (defaultPackage == null)
            {
                defaultPackage = YooAssets.CreatePackage(packageName);
                YooAssets.SetDefaultPackage(defaultPackage);
            }

            // 编辑器下的模拟模式
            InitializationOperation initializationOperation = null;
            if (playMode == EPlayMode.EditorSimulateMode)
            {
                var createParameters = new EditorSimulateModeParameters();
                createParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(packageName);
                initializationOperation = defaultPackage.InitializeAsync(createParameters);
            }
            else if (playMode == EPlayMode.OfflinePlayMode)
            {
                var createParameters = new OfflinePlayModeParameters();
                initializationOperation = defaultPackage.InitializeAsync(createParameters);
            }
            else if (playMode == EPlayMode.HostPlayMode)
            {
                string defaultHostServer = GetHostServerURL();
                string fallbackHostServer = GetHostServerURL();
                HostPlayModeParameters createParameters = new();
                createParameters.QueryServices = new GameQueryServices();
                createParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                initializationOperation = defaultPackage.InitializeAsync(createParameters);
            }

            await initializationOperation.Task;

            if (defaultPackage.InitializeStatus != EOperationStatus.Succeed)
            {
                Debug.LogError($"{initializationOperation.Error}");
            }
        }

        private async ETTask LoadGlobalConfigAsync()
        {
            AssetOperationHandle handler = YooAssets.LoadAssetAsync<GlobalConfig>("GlobalConfig");
            await handler;
            GlobalConfig.Instance = handler.AssetObject as GlobalConfig;
            handler.Release();
            defaultPackage.UnloadUnusedAssets();
        }

        public byte[] LoadRawFile(string location)
        {
            RawFileOperationHandle handle = YooAssets.LoadRawFileSync(location);
            return handle.GetRawFileData();
        }
        
        public T LoadAsset<T>(string location) where T : UnityEngine.Object
        {
            // self.AssetsOperationHandles.TryGetValue(location, out AssetOperationHandle handle);
            AssetOperationHandle handle;
            // if (handle == null)
            {
                handle = YooAssets.LoadAssetSync<T>(location);
                // self.AssetsOperationHandles[location] = handle;
            }

            return handle.AssetObject as T;
        }

        public async ETTask<byte[]> LoadRawFileAsync(string location)
        {
            RawFileOperationHandle handle = YooAssets.LoadRawFileAsync(location);
            await handle;
            return handle.GetRawFileData();
        }

        public async ETTask<T> LoadAssetAsync<T>(string location) where T : UnityEngine.Object
        {
            AssetOperationHandle handle = YooAssets.LoadAssetAsync<T>(location);
            await handle.Task;
            T t = (T)handle.AssetObject;
            handle.Release();
            return t;
        }

        public string[] GetAddressesByTag(string tag)
        {
            AssetInfo[] assetInfos = YooAssets.GetAssetInfos(tag);
            string[] addresses = new string[assetInfos.Length];
            for (int i = 0; i < assetInfos.Length; i++)
            {
                addresses[i] = assetInfos[i].Address;
            }

            return addresses;
        }

        /// <summary>
        /// 获取资源服务器地址
        /// </summary>
        private string GetHostServerURL()
        {
            string hostServerIP = "http://8.218.236.160"; //远程资源服务器地址
            //string hostServerIP = "http://127.0.0.1";
            string gameVersion = "v1.0";

#if UNITY_EDITOR
            if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
                return $"{hostServerIP}/CDN/Android/{gameVersion}";
            else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
                return $"{hostServerIP}/CDN/IPhone/{gameVersion}";
            else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)
                return $"{hostServerIP}/CDN/WebGL/{gameVersion}";
            else
                return $"{hostServerIP}/CDN/PC/{gameVersion}";
#else
		if (Application.platform == RuntimePlatform.Android)
			return $"{hostServerIP}/CDN/Android/{gameVersion}";
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
			return $"{hostServerIP}/CDN/IPhone/{gameVersion}";
		else if (Application.platform == RuntimePlatform.WebGLPlayer)
			return $"{hostServerIP}/CDN/WebGL/{gameVersion}";
		else
			return $"{hostServerIP}/CDN/PC/{gameVersion}";
#endif
        }

        public void Destroy()
        {
            YooAssets.Destroy();
        }

        /// <summary>
        /// 内置文件查询服务类
        /// </summary>
        private class GameQueryServices : IQueryServices
        {
            /*public bool QueryStreamingAssets(string fileName)
            {
                // 注意：使用了BetterStreamingAssets插件，使用前需要初始化该插件！
                string buildinFolderName = YooAssets.GetStreamingAssetBuildinFolderName();
                return StreamingAssetsHelper.FileExists($"{buildinFolderName}/{fileName}");
            }*/
            public bool QueryStreamingAssets(string packageName, string fileName)
            {
                // 注意：fileName包含文件格式
                string filePath = Path.Combine(YooAssetSettings.DefaultYooFolderName, packageName, fileName);
                return BetterStreamingAssets.FileExists(filePath);
            }
        }

        /// <summary>
        /// 资源文件解密服务类
        /// </summary>
        private class GameDecryptionServices : IDecryptionServices
        {
            public ulong LoadFromFileOffset(DecryptFileInfo fileInfo)
            {
                return 32;
            }

            public byte[] LoadFromMemory(DecryptFileInfo fileInfo)
            {
                throw new NotImplementedException();
            }

            public Stream LoadFromStream(DecryptFileInfo fileInfo)
            {
                BundleStream bundleStream = new BundleStream(fileInfo.FilePath, FileMode.Open);
                return bundleStream;
            }

            public uint GetManagedReadBufferSize()
            {
                return 1024;
            }
        }
    }

    /// <summary>
    /// 远端资源地址查询服务类
    /// </summary>
    public class RemoteServices : IRemoteServices
    {
        private readonly string _defaultHostServer;
        private readonly string _fallbackHostServer;

        public RemoteServices(string defaultHostServer, string fallbackHostServer)
        {
            _defaultHostServer = defaultHostServer;
            _fallbackHostServer = fallbackHostServer;
        }
        string IRemoteServices.GetRemoteMainURL(string fileName)
        {
            return $"{_defaultHostServer}/{fileName}";
        }
        string IRemoteServices.GetRemoteFallbackURL(string fileName)
        {
            return $"{_fallbackHostServer}/{fileName}";
        }
    }

}
