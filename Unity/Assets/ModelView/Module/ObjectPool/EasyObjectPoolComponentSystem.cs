using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ET
{
    public static class EasyObjectPoolComponentSystem
    {
        public static void InitPool(this EasyObjectPoolComponent self,  string poolName, int size, PoolInflationType type = PoolInflationType.DOUBLE)
        {
            if (self.poolDict.ContainsKey(poolName))
            {
                return;
            }
            else
            {
                try
                {
                    GameObject pb = self.GetGameObjectByResType(poolName);
                    if (pb == null)
                    {
                        Debug.LogError("[ResourceManager] Invalide prefab name for pooling :" + poolName);
                        return;
                    }
                    self.poolDict[poolName] = new Pool(poolName, pb, Game.Scene.GetComponent<GlobalComponent>().PoolRoot.gameObject, size, type);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
        
        public static async ETTask InitPoolFormGamObjectAsync(this EasyObjectPoolComponent self,  GameObject pb, int size, PoolInflationType type = PoolInflationType.DOUBLE)
        {
            string poolName = pb.name;
            if (self.poolDict.ContainsKey(poolName))
            {
                return;
            }
            else
            {
                try
                {
                    if (pb == null)
                    {
                        Debug.LogError("[ResourceManager] Invalide prefab name for pooling :" + poolName);
                        return;
                    }
                    self.poolDict[poolName] = new Pool(poolName, pb, Game.Scene.GetComponent<GlobalComponent>().PoolRoot.gameObject, size, type);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }

            await ETTask.CompletedTask;
        }
        
        
        public static async ETTask InitPoolAsync(this EasyObjectPoolComponent self,  string poolName, int size , PoolInflationType type = PoolInflationType.DOUBLE)
        {
            if (self.poolDict.ContainsKey(poolName))
            {
                return;
            }
            else
            {
                try
                {
                    GameObject pb = await self.GetGameObjectByResTypeAsync(poolName);
                    if (pb == null)
                    {
                        Debug.LogError("[ResourceManager] Invalide prefab name for pooling :" + poolName);
                        return;
                    }
                    self.poolDict[poolName] = new Pool(poolName, pb, Game.Scene.GetComponent<GlobalComponent>().PoolRoot.gameObject, size, type);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
        
        
        
        /// <summary>
        /// Returns an available object from the pool 
        /// OR null in case the pool does not have any object available & can grow size is false.
        /// </summary>
        /// <OtherParam name="poolName"></OtherParam>
        /// <returns></returns>
        public static GameObject GetObjectFromPool(this EasyObjectPoolComponent self, string poolName,    bool autoActive = true, int autoCreate = 0)
        {
            GameObject result = null;

            if (!self.poolDict.ContainsKey(poolName) && autoCreate > 0)
            {
                self.InitPool(poolName, autoCreate, PoolInflationType.INCREMENT);
            }

            if (self.poolDict.ContainsKey(poolName))
            {
                Pool pool = self.poolDict[poolName];
                result = pool.NextAvailableObject(autoActive);
                //scenario when no available object is found in pool
#if UNITY_EDITOR
                if (result == null)
                {
                    Debug.LogWarning("[ResourceManager]:No object available in " + poolName);
                }
#endif
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError("[ResourceManager]:Invalid pool name specified: " + poolName);
            }
#endif
            return result;
        }

        
        
        /// <summary>
        /// Returns an available object from the pool 
        /// OR null in case the pool does not have any object available & can grow size is false.
        /// </summary>
        /// <OtherParam name="poolName"></OtherParam>
        /// <returns></returns>
        public static async ETTask<GameObject> GetObjectFromPoolAsync(this EasyObjectPoolComponent self, string poolName,  bool autoActive = true, int autoCreate = 0)
        {
            
            GameObject result = null;

            if (!self.poolDict.ContainsKey(poolName) && autoCreate > 0)
            {
               await  self.InitPoolAsync(poolName, autoCreate, PoolInflationType.INCREMENT);
            }

            if (self.poolDict.ContainsKey(poolName))
            {
                Pool pool = self.poolDict[poolName];
                result = await pool.NextAvailableObjectAsync(autoActive);
                //scenario when no available object is found in pool
#if UNITY_EDITOR
                if (result == null)
                {
                    Debug.LogWarning("[ResourceManager]:No object available in " + poolName);
                }
#endif
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError("[ResourceManager]:Invalid pool name specified: " + poolName);
            }
#endif
            return result;
        }
        
        
        
        /// <summary>
        /// Return obj to the pool
        /// </summary>
        /// <OtherParam name="go"></OtherParam>
        public static void ReturnObjectToPool(this EasyObjectPoolComponent self, GameObject go)
        {
            PoolObject po = go.GetComponent<PoolObject>();
            if (po == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Specified object is not a pooled instance: " + go.name);
#endif
            }
            else
            {
                Pool pool = null;
                if (self.poolDict.TryGetValue(po.poolName, out pool))
                {
                    pool.ReturnObjectToPool(po);
                }
#if UNITY_EDITOR
                else
                {
                    Debug.LogWarning("No pool available with name: " + po.poolName);
                }
#endif
            }
        }

        /// <summary>
        /// Return obj to the pool
        /// </summary>
        /// <OtherParam name="t"></OtherParam>
        public static void ReturnTransformToPool(this EasyObjectPoolComponent self,  Transform t)
        {
            if (t == null)
            {
#if UNITY_EDITOR
                Debug.LogError("[ResourceManager] try to return a null transform to pool!");
#endif
                return;
            }
            self.ReturnObjectToPool(t.gameObject);
        }

        public static GameObject GetGameObjectByResType(this EasyObjectPoolComponent self, string poolName)
        {
            GameObject pb = null;
            Game.Scene.GetComponent<ResourcesComponent>().LoadBundle(poolName.StringToAB());
            pb = Game.Scene.GetComponent<ResourcesComponent>().GetAsset(poolName.StringToAB(), poolName ) as GameObject;
            return pb;
        }
        
        public static async ETTask<GameObject> GetGameObjectByResTypeAsync(this EasyObjectPoolComponent self, string poolName)
        {
            GameObject pb = null;
            await Game.Scene.GetComponent<ResourcesComponent>().LoadBundleAsync(poolName.StringToAB());
            pb = Game.Scene.GetComponent<ResourcesComponent>().GetAsset(poolName.StringToAB(), poolName ) as GameObject;
            return pb;
        }        
        
        
        
    }
}