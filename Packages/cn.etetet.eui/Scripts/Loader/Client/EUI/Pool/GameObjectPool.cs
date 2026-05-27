using System.Collections.Generic;
using UnityEngine;

namespace ET.Client
{
    /// <summary>
    /// 轻量 GameObject 对象池：每个 poolName 对应一个 GameObjectPool。
    /// 移植自 marchingbytes EasyObjectPool（原 ET.Client.GameObjectPool）。
    /// </summary>
    public class GameObjectPool
    {
        private readonly Stack<PoolObject> availableObjStack = new Stack<PoolObject>();

        private GameObject rootObj;
        private readonly PoolInflationType inflationType;
        private readonly string poolName;
        private int objectsInUse;

        public GameObjectPool(string poolName, GameObject poolObjectPrefab, GameObject rootPoolObj, int initialCount, PoolInflationType type)
        {
            if (poolObjectPrefab == null)
            {
                Log.Error("[GameObjectPool] null pool object prefab!");
                return;
            }
            this.poolName = poolName;
            this.inflationType = type;
            this.rootObj = new GameObject(poolName + "Pool");
            this.rootObj.transform.SetParent(rootPoolObj.transform, false);

            GameObject go = UnityEngine.Object.Instantiate(poolObjectPrefab);
            PoolObject po = go.GetComponent<PoolObject>();
            if (po == null)
            {
                po = go.AddComponent<PoolObject>();
            }
            po.poolName = poolName;
            AddObjectToPool(po);

            PopulatePool(Mathf.Max(initialCount, 1));
        }

        private void AddObjectToPool(PoolObject po)
        {
            po.gameObject.SetActive(false);
            po.gameObject.name = poolName;
            availableObjStack.Push(po);
            po.isPooled = true;
            po.gameObject.transform.SetParent(rootObj.transform, false);
        }

        private void PopulatePool(int initialCount)
        {
            for (int index = 0; index < initialCount; index++)
            {
                PoolObject po = UnityEngine.Object.Instantiate(availableObjStack.Peek());
                AddObjectToPool(po);
            }
        }

        public GameObject NextAvailableObject(bool autoActive)
        {
            PoolObject po = null;
            if (availableObjStack.Count > 1)
            {
                po = availableObjStack.Pop();
            }
            else
            {
                int increaseSize = 0;
                if (inflationType == PoolInflationType.INCREMENT)
                {
                    increaseSize = 1;
                }
                else if (inflationType == PoolInflationType.DOUBLE)
                {
                    increaseSize = availableObjStack.Count + Mathf.Max(objectsInUse, 0);
                }
                if (increaseSize > 0)
                {
                    PopulatePool(increaseSize);
                    po = availableObjStack.Pop();
                }
            }

            GameObject result = null;
            if (po != null)
            {
                objectsInUse++;
                po.isPooled = false;
                result = po.gameObject;
                if (autoActive)
                {
                    result.SetActive(true);
                }
            }
            return result;
        }

        public void ReturnObjectToPool(PoolObject po)
        {
            if (poolName.Equals(po.poolName))
            {
                objectsInUse--;
                if (!po.isPooled)
                {
                    AddObjectToPool(po);
                }
            }
            else
            {
                Log.Error($"Trying to add object to incorrect pool {po.poolName} {poolName}");
            }
        }
    }
}
