using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using YooAsset;

namespace ET.Client
{
    [EntitySystemOf(typeof(ResourcesLoaderComponent))]
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
                switch (kv.Value)
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
        }

        public static async ETTask<T> LoadAssetAsync<T>(this ResourcesLoaderComponent self, string location) where T : UnityEngine.Object
        {
            EntityRef<ResourcesLoaderComponent> selfRef = self;
            using var _ = await self.Root().CoroutineLockComponent.Wait(CoroutineLockType.ResourcesLoader, location.GetHashCode());

            HandleBase handler;
            self = selfRef;
            if (!self.handlers.TryGetValue(location, out handler))
            {
                AssetHandle assetHandle = self.package.LoadAssetAsync<T>(location);
                await assetHandle;

                self = selfRef;
                handler = assetHandle;
                self.handlers.Add(location, handler);
            }

            return (T)((AssetHandle)handler).AssetObject;
        }

        public static T LoadAssetSync<T>(this ResourcesLoaderComponent self, string location) where T : UnityEngine.Object
        {
            HandleBase handler;
            if (!self.handlers.TryGetValue(location, out handler))
            {
                handler = self.package.LoadAssetSync<T>(location);
                self.handlers.Add(location, handler);
            }

            return (T)((AssetHandle)handler).AssetObject;
        }

        public static async ETTask<Dictionary<string, T>> LoadAllAssetsAsync<T>(this ResourcesLoaderComponent self, string location) where T : UnityEngine.Object
        {
            EntityRef<ResourcesLoaderComponent> selfRef = self;
            using var _ = await self.Root().CoroutineLockComponent.Wait(CoroutineLockType.ResourcesLoader, location.GetHashCode());

            HandleBase handler;
            self = selfRef;
            if (!self.handlers.TryGetValue(location, out handler))
            {
                AllAssetsHandle allHandle = self.package.LoadAllAssetsAsync<T>(location);
                await allHandle;

                self = selfRef;
                handler = allHandle;
                self.handlers.Add(location, handler);
            }

            Dictionary<string, T> dictionary = new();
            foreach (UnityEngine.Object assetObj in ((AllAssetsHandle)handler).AllAssetObjects)
            {
                T t = assetObj as T;
                dictionary.Add(t.name, t);
            }

            return dictionary;
        }

        public static async ETTask LoadSceneAsync(this ResourcesLoaderComponent self, string location, LoadSceneMode loadSceneMode, Action<float> action = null)
        {
            EntityRef<ResourcesLoaderComponent> selfRef = self;
            using var _ = await self.Root().CoroutineLockComponent.Wait(CoroutineLockType.ResourcesLoader, location.GetHashCode());

            HandleBase handler;
            self = selfRef;
            if (self.handlers.TryGetValue(location, out handler))
            {
                return;
            }

            SceneHandle sceneHandle = self.package.LoadSceneAsync(location, loadSceneMode);
            handler = sceneHandle;
            self.handlers.Add(location, handler);

            await ETTask.WaitAll(new[] { WaitLoadFinish(sceneHandle), LoadProgressCallback(sceneHandle) });

            return;

            async ETTask WaitLoadFinish(SceneHandle sh)
            {
                await sh;
            }

            async ETTask LoadProgressCallback(SceneHandle sh)
            {
                self = selfRef;
                TimerComponent timerComponent = self.Root().TimerComponent;
                while (true)
                {
                    await timerComponent.WaitAsync(500);
                    float progress = sh.Progress;
                    action?.Invoke(progress);
                    if (progress >= 1)
                    {
                        return;
                    }
                }
            }
        }
    }
}
