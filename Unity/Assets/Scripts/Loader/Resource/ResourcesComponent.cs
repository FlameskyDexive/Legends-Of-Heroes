using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YooAsset;

namespace ET
{

    // 用于字符串转换，减少GC
    [FriendOf(typeof(ResourcesComponent))]
    public static class AssetBundleHelper
    {
        public static string StringToAB(this string value)
        {
            // string result =  $"Assets/Bundles/UI/Dlg/{value}.prefab";
            string result = $"{value}.prefab";
            return result;
        }

    }


    /// <summary>
    /// 远端资源地址查询服务类
    /// </summary>
    public class RemoteServices : IRemoteService
    {
        private readonly string _defaultHostServer;
        private readonly string _fallbackHostServer;

        public RemoteServices(string defaultHostServer, string fallbackHostServer)
        {
            _defaultHostServer = defaultHostServer;
            _fallbackHostServer = fallbackHostServer;
        }
        public IReadOnlyList<string> GetRemoteUrls(string fileName)
        {
            return new[]
            {
                $"{_defaultHostServer}/{fileName}",
                $"{_fallbackHostServer}/{fileName}"
            };
        }
    }

    public class ResourcesComponent : Singleton<ResourcesComponent>, ISingletonAwake
    {
        private ResourcePackage defaultPackage;
        public void Awake()
        {
            YooAssets.Initialize();
            BetterStreamingAssets.Initialize();
            // this.LoadGlobalConfigAsync().Coroutine();
        }

        protected override void Destroy()
        {
            YooAssets.Destroy();
        }
        
        private IEnumerator LoadGlobalConfig()
        {
            AssetHandle handler = defaultPackage.LoadAssetAsync<GlobalConfig>("GlobalConfig");
            yield return handler;
            GlobalConfig.Instance = handler.AssetObject as GlobalConfig;
            handler.Release();
            defaultPackage.UnloadUnusedAssetsAsync();
        }

        private async ETTask LoadGlobalConfigAsync()
        {
            AssetHandle handler = defaultPackage.LoadAssetAsync<GlobalConfig>("GlobalConfig");
            await handler;
            GlobalConfig.Instance = handler.AssetObject as GlobalConfig;
            handler.Release();
            defaultPackage.UnloadUnusedAssetsAsync();
        }

        public async ETTask RestartAsync()
        {
            await this.LoadGlobalConfigAsync();
        }

        public async ETTask CreatePackageAsync(string packageName, bool isDefault = false)
        {
            if (!YooAssets.TryGetPackage(packageName, out defaultPackage))
            {
                defaultPackage = YooAssets.CreatePackage(packageName);
            }

            EPlayMode ePlayMode = Define.PlayMode;

            // 编辑器下的模拟模式
            switch (ePlayMode)
            {
                case EPlayMode.EditorSimulateMode:
                    {
                        var buildResult = EditorSimulateBuildInvoker.Build(packageName, (int)EBundleType.AssetBundle);
                        EditorSimulateModeOptions createParameters = new();
                        createParameters.EditorFileSystemParameters = FileSystemParameters.CreateDefaultEditorFileSystemParameters(buildResult.PackageRootDirectory);
                        await defaultPackage.InitializePackageAsync(createParameters);
                        break;
                    }
                case EPlayMode.OfflinePlayMode:
                    {
                        OfflinePlayModeOptions createParameters = new();
                        createParameters.BuiltinFileSystemParameters = FileSystemParameters.CreateDefaultBuiltinFileSystemParameters();
                        createParameters.BuiltinFileSystemParameters.AddParameter(EFileSystemParameter.AssetbundleDecryptor, new FileOffsetDecryption());
                        await defaultPackage.InitializePackageAsync(createParameters);
                        break;
                    }
                case EPlayMode.HostPlayMode:
                    {
                        string defaultHostServer = GetHostServerURL();
                        string fallbackHostServer = GetHostServerURL();
                        RemoteServices remoteServices = new(defaultHostServer, fallbackHostServer);
                        HostPlayModeOptions createParameters = new();
                        createParameters.BuiltinFileSystemParameters = FileSystemParameters.CreateDefaultBuiltinFileSystemParameters();
                        createParameters.BuiltinFileSystemParameters.AddParameter(EFileSystemParameter.CopyBuiltinPackageManifest, true);
                        createParameters.BuiltinFileSystemParameters.AddParameter(EFileSystemParameter.AssetbundleDecryptor, new FileOffsetDecryption());
                        createParameters.CacheFileSystemParameters = FileSystemParameters.CreateDefaultSandboxFileSystemParameters(remoteServices);
                        createParameters.CacheFileSystemParameters.AddParameter(EFileSystemParameter.AssetbundleDecryptor, new FileOffsetDecryption());
                        await defaultPackage.InitializePackageAsync(createParameters);
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await this.LoadGlobalConfigAsync();

            return;

            string GetHostServerURL()
            {
                //string hostServerIP = "http://10.0.2.2"; //安卓模拟器地址
                string hostServerIP = "http://127.0.0.1";
                string appVersion = "v1.0";

#if UNITY_EDITOR
                if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
                {
                    return $"{hostServerIP}/CDN/Android/{appVersion}";
                }
                else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
                {
                    return $"{hostServerIP}/CDN/IPhone/{appVersion}";
                }
                else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)
                {
                    return $"{hostServerIP}/CDN/WebGL/{appVersion}";
                }

                return $"{hostServerIP}/CDN/PC/{appVersion}";
#else
            if (Application.platform == RuntimePlatform.Android)
            {
                return $"{hostServerIP}/CDN/Android/{appVersion}";
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return $"{hostServerIP}/CDN/IPhone/{appVersion}";
            }
            else if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                return $"{hostServerIP}/CDN/WebGL/{appVersion}";
            }

            return $"{hostServerIP}/CDN/PC/{appVersion}";
#endif
            }

        }

        public void DestroyPackage(string packageName)
        {
            ResourcePackage package = YooAssets.GetPackage(packageName);
            package.UnloadUnusedAssetsAsync();
        }

        /// <summary>
        /// 主要用来加载dll config aotdll，因为这时候纤程还没创建，无法使用ResourcesLoaderComponent。
        /// 游戏中的资源应该使用ResourcesLoaderComponent来加载
        /// </summary>
        public T LoadAssetSync<T>(string location) where T : UnityEngine.Object
        {
            AssetHandle handle = defaultPackage.LoadAssetSync<T>(location);
            T t = (T)handle.AssetObject;
            handle.Release();
            return t;
        }

        public byte[] LoadRawFile(string location)
        {
            RawFileHandle handle = defaultPackage.LoadRawFileSync(location);
            byte[] bytes = File.ReadAllBytes(handle.GetRawFilePath());
            handle.Release();
            return bytes;
        }

        public T LoadAsset<T>(string location) where T : UnityEngine.Object
        {
            // self.AssetsOperationHandles.TryGetValue(location, out AssetOperationHandle handle);
            AssetHandle handle;
            // if (handle == null)
            {
                handle = defaultPackage.LoadAssetSync<T>(location);
                // self.AssetsOperationHandles[location] = handle;
            }

            return handle.AssetObject as T;
        }

        public async ETTask<byte[]> LoadRawFileAsync(string location)
        {
            RawFileHandle handle = defaultPackage.LoadRawFileAsync(location);
            await handle;
            byte[] bytes = File.ReadAllBytes(handle.GetRawFilePath());
            handle.Release();
            return bytes;
        }

        /// <summary>
        /// 主要用来加载dll config aotdll，因为这时候纤程还没创建，无法使用ResourcesLoaderComponent。
        /// 游戏中的资源应该使用ResourcesLoaderComponent来加载
        /// </summary>
        public async ETTask<T> LoadAssetAsync<T>(string location) where T : UnityEngine.Object
        {
            AssetHandle handle = defaultPackage.LoadAssetAsync<T>(location);
            await handle;
            T t = (T)handle.AssetObject;
            handle.Release();
            return t;
        }

        /// <summary>
        /// 主要用来加载dll config aotdll，因为这时候纤程还没创建，无法使用ResourcesLoaderComponent。
        /// 游戏中的资源应该使用ResourcesLoaderComponent来加载
        /// </summary>
        public async ETTask<Dictionary<string, T>> LoadAllAssetsAsync<T>(string location) where T : UnityEngine.Object
        {
            AllAssetsHandle allAssetsOperationHandle = defaultPackage.LoadAllAssetsAsync<T>(location);
            await allAssetsOperationHandle;
            Dictionary<string, T> dictionary = new Dictionary<string, T>();
            foreach (UnityEngine.Object assetObj in allAssetsOperationHandle.AllAssetObjects)
            {
                T t = assetObj as T;
                dictionary.Add(t.name, t);
            }
            allAssetsOperationHandle.Release();
            return dictionary;
        }
    }
}
