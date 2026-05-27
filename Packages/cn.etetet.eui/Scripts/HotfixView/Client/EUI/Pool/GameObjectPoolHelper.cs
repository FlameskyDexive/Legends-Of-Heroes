using System;
using UnityEngine;

namespace ET.Client
{
    /// <summary>
    /// EUI 全局 GameObject 池入口。静态状态保存在 <see cref="GameObjectPoolStore"/>（Loader 端）。
    /// 通过 Scene 上的 ResourcesLoaderComponent 加载预制体。
    /// </summary>
    public static class GameObjectPoolHelper
    {
        /// <summary>
        /// 静态绑定：池根由 Init 场景固化（<see cref="GameObjectPoolStore.POOL_ROOT_PATH"/>），首次访问 Find 一次后缓存。
        /// 若场景里没有该节点则报错；不再动态创建 / DontDestroyOnLoad。
        /// </summary>
        private static GameObject GetPoolRoot()
        {
            if (GameObjectPoolStore.PoolRoot != null)
            {
                return GameObjectPoolStore.PoolRoot;
            }
            var existing = GameObject.Find(GameObjectPoolStore.POOL_ROOT_PATH);
            if (existing == null)
            {
                Log.Error($"[GameObjectPoolHelper] 找不到场景节点: {GameObjectPoolStore.POOL_ROOT_PATH}，请确认 Init 场景已挂好 PoolRoot");
                return null;
            }
            GameObjectPoolStore.PoolRoot = existing;
            return GameObjectPoolStore.PoolRoot;
        }

        public static void InitPool(Scene scene, string poolName, int size, PoolInflationType type = PoolInflationType.DOUBLE)
        {
            if (GameObjectPoolStore.PoolDict.ContainsKey(poolName))
            {
                return;
            }
            try
            {
                var loader = scene.GetComponent<ResourcesLoaderComponent>();
                if (loader == null)
                {
                    Log.Error("[GameObjectPoolHelper] scene has no ResourcesLoaderComponent");
                    return;
                }
                GameObject pb = loader.LoadAssetSync<GameObject>(poolName);
                if (pb == null)
                {
                    Log.Error("[GameObjectPoolHelper] invalid prefab name for pooling: " + poolName);
                    return;
                }
                var poolRoot = GetPoolRoot();
                if (poolRoot == null)
                {
                    return;
                }
                GameObjectPoolStore.PoolDict[poolName] = new GameObjectPool(poolName, pb, poolRoot, size, type);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public static async ETTask InitPoolWithPathAsync(Scene scene, string poolName, string assetLocation, int size, PoolInflationType type = PoolInflationType.DOUBLE)
        {
            if (GameObjectPoolStore.PoolDict.ContainsKey(poolName))
            {
                return;
            }
            try
            {
                var loader = scene.GetComponent<ResourcesLoaderComponent>();
                if (loader == null)
                {
                    Log.Error("[GameObjectPoolHelper] scene has no ResourcesLoaderComponent");
                    return;
                }
                GameObject pb = await loader.LoadAssetAsync<GameObject>(assetLocation);
                if (pb == null)
                {
                    Log.Error("[GameObjectPoolHelper] invalid prefab name for pooling: " + poolName);
                    return;
                }
                var poolRoot = GetPoolRoot();
                if (poolRoot == null)
                {
                    return;
                }
                GameObjectPoolStore.PoolDict[poolName] = new GameObjectPool(poolName, pb, poolRoot, size, type);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public static GameObject GetObjectFromPool(string poolName, bool autoActive = true)
        {
            if (!GameObjectPoolStore.PoolDict.TryGetValue(poolName, out GameObjectPool pool))
            {
                Log.Error("[GameObjectPoolHelper] invalid pool name specified: " + poolName);
                return null;
            }
            return pool.NextAvailableObject(autoActive);
        }

        public static GameObject GetObjectFromPool(Scene scene, string poolName, bool autoActive = true, int autoCreate = 0)
        {
            if (!GameObjectPoolStore.PoolDict.ContainsKey(poolName) && autoCreate > 0)
            {
                InitPool(scene, poolName, autoCreate, PoolInflationType.INCREMENT);
            }
            return GetObjectFromPool(poolName, autoActive);
        }

        public static async ETTask<GameObject> GetObjectFromPoolAsync(Scene scene, string poolName, string assetLocation, bool autoActive = true, int autoCreate = 0)
        {
            if (!GameObjectPoolStore.PoolDict.ContainsKey(poolName) && autoCreate > 0)
            {
                await InitPoolWithPathAsync(scene, poolName, assetLocation, autoCreate, PoolInflationType.INCREMENT);
            }
            return GetObjectFromPool(poolName, autoActive);
        }

        public static void ReturnObjectToPool(GameObject go)
        {
            if (go == null)
            {
                return;
            }
            PoolObject po = go.GetComponent<PoolObject>();
            if (po == null)
            {
                Log.Warning("specified object is not a pooled instance: " + go.name);
                return;
            }
            if (GameObjectPoolStore.PoolDict.TryGetValue(po.poolName, out GameObjectPool pool))
            {
                pool.ReturnObjectToPool(po);
            }
            else
            {
                Log.Warning("no pool available with name: " + po.poolName);
            }
        }

        public static void ReturnTransformToPool(Transform t)
        {
            if (t == null)
            {
                Log.Error("[GameObjectPoolHelper] return null transform to pool");
                return;
            }
            ReturnObjectToPool(t.gameObject);
        }
    }
}
