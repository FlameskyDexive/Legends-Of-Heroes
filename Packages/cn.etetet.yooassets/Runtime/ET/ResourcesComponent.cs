using System;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace ET
{
    /// <summary>
    /// 远端资源地址查询服务类（YooAsset 3.0 IRemoteService）
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

    [AllowInstance]
    public class ResourcesComponent : Singleton<ResourcesComponent>, ISingletonAwake
    {
        // YooAsset 3.0 移除了 SetDefaultPackage / 全局 YooAssets.LoadAssetAsync。
        // 通过本字段缓存"默认包"以便 LoadAsset*Async 的便捷入口继续可用。
        private ResourcePackage defaultPackage;

        public void Awake()
        {
            YooAssets.Initialize();
        }

        protected override void Destroy()
        {
            YooAssets.Destroy();
        }

        public async ETTask CreatePackageAsync(string packageName, bool isDefault = false)
        {
            YooConfig yooConfig = Resources.Load<YooConfig>("YooConfig");
            if (yooConfig == null)
            {
                Log.Error("Resources.Load<YooConfig>(\"YooConfig\") 返回 null，请确认 cn.etetet.yooassets/Resources/YooConfig.asset 存在且脚本引用未丢失");
                return;
            }

            if (!YooAssets.TryGetPackage(packageName, out ResourcePackage package))
            {
                package = YooAssets.CreatePackage(packageName);
            }

            if (isDefault)
            {
                this.defaultPackage = package;
            }

            switch (yooConfig.EPlayMode)
            {
                case EPlayMode.EditorSimulateMode:
                {
                    var buildResult = EditorSimulateBuildInvoker.Build(packageName, (int)EBundleType.VirtualBundle);
                    EditorSimulateModeOptions createParameters = new();
                    createParameters.EditorFileSystemParameters = FileSystemParameters.CreateDefaultEditorFileSystemParameters(buildResult.PackageRootDirectory);
                    await package.InitializePackageAsync(createParameters);
                    break;
                }
                case EPlayMode.OfflinePlayMode:
                {
                    OfflinePlayModeOptions createParameters = new();
                    createParameters.BuiltinFileSystemParameters = FileSystemParameters.CreateDefaultBuiltinFileSystemParameters();
                    await package.InitializePackageAsync(createParameters);
                    break;
                }
                case EPlayMode.HostPlayMode:
                {
                    string defaultHostServer = GetHostServerURL(yooConfig.Url, package.PackageName);
                    string fallbackHostServer = GetHostServerURL(yooConfig.Url, package.PackageName);
                    IRemoteService remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                    HostPlayModeOptions createParameters = new();
                    createParameters.BuiltinFileSystemParameters = FileSystemParameters.CreateDefaultBuiltinFileSystemParameters();
                    createParameters.CacheFileSystemParameters = FileSystemParameters.CreateDefaultSandboxFileSystemParameters(remoteServices);
                    await package.InitializePackageAsync(createParameters);
                    break;
                }
                case EPlayMode.WebPlayMode:
                {
                    string defaultHostServer = GetHostServerURL(yooConfig.Url, package.PackageName);
                    string fallbackHostServer = GetHostServerURL(yooConfig.Url, package.PackageName);
                    IRemoteService remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                    WebPlayModeOptions createParameters = new();
                    createParameters.WebServerFileSystemParameters = FileSystemParameters.CreateDefaultWebServerFileSystemParameters();
                    createParameters.WebRemoteFileSystemParameters = FileSystemParameters.CreateDefaultWebRemoteFileSystemParameters(remoteServices);
                    await package.InitializePackageAsync(createParameters);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            RequestPackageVersionOperation versionOperation = package.RequestPackageVersionAsync();
            await versionOperation;
            if (versionOperation.Status != EOperationStatus.Succeeded)
            {
                Log.Error($"YooAsset RequestPackageVersion failed: {packageName} {versionOperation.Error}");
                return;
            }

            LoadPackageManifestOperation manifestOperation = package.LoadPackageManifestAsync(
                new LoadPackageManifestOptions(versionOperation.PackageVersion, 60));
            await manifestOperation;
            if (manifestOperation.Status != EOperationStatus.Succeeded)
            {
                Log.Error($"YooAsset LoadPackageManifest failed: {packageName} {manifestOperation.Error}");
            }
        }

        private static string GetHostServerURL(string url, string packageName)
        {
            string hostServerIP = url;
            string appVersion = "v1.0";

#if UNITY_EDITOR
            switch (UnityEditor.EditorUserBuildSettings.activeBuildTarget)
            {
                case UnityEditor.BuildTarget.Android:
                    return $"{hostServerIP}/CDN/Android/{appVersion}";
                case UnityEditor.BuildTarget.iOS:
                    return $"{hostServerIP}/CDN/IPhone/{appVersion}";
                case UnityEditor.BuildTarget.WebGL:
                    return $"{hostServerIP}/StreamingAssets/Bundles/{packageName}";
                default:
                    return $"{hostServerIP}/CDN/PC/{appVersion}";
            }
#else
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return $"{hostServerIP}/CDN/Android/{appVersion}";
                case RuntimePlatform.IPhonePlayer:
                    return $"{hostServerIP}/CDN/IPhone/{appVersion}";
                case RuntimePlatform.WebGLPlayer:
                    return $"{hostServerIP}/StreamingAssets/Bundles/{packageName}";
                default:
                    return $"{hostServerIP}/CDN/PC/{appVersion}";
            }
#endif
        }

        public async ETTask DestroyPackage(string packageName)
        {
            ResourcePackage package = YooAssets.GetPackage(packageName);
            await package.DestroyPackageAsync();
        }

        /// <summary>
        /// 主要用来加载 dll / config / aotdll，这时纤程还没创建，无法使用 ResourcesLoaderComponent。
        /// 游戏中的资源应该使用 ResourcesLoaderComponent 来加载。
        /// </summary>
        public async ETTask<T> LoadAssetAsync<T>(string location) where T : UnityEngine.Object
        {
            if (this.defaultPackage == null)
            {
                Log.Error("ResourcesComponent.LoadAssetAsync 调用时默认包未初始化，请先 CreatePackageAsync(name, isDefault: true)。");
                return null;
            }
            AssetHandle handle = this.defaultPackage.LoadAssetAsync<T>(location);
            await handle;
            T t = (T)handle.AssetObject;
            handle.Release();
            return t;
        }

        /// <summary>
        /// 主要用来加载 dll / config / aotdll。
        /// </summary>
        public async ETTask<Dictionary<string, T>> LoadAllAssetsAsync<T>(string location) where T : UnityEngine.Object
        {
            if (this.defaultPackage == null)
            {
                Log.Error("ResourcesComponent.LoadAllAssetsAsync 调用时默认包未初始化，请先 CreatePackageAsync(name, isDefault: true)。");
                return null;
            }
            AllAssetsHandle allAssetsOperationHandle = this.defaultPackage.LoadAllAssetsAsync<T>(location);
            await allAssetsOperationHandle;
            Dictionary<string, T> dictionary = new();
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
