using System;
using ET;
using UnityEngine;

namespace UnityEngine.UI
{
    public interface LoopScrollPrefabSource
    {
        GameObject GetObject(int index);

        void ReturnObject(Transform trans, bool isDestroy = false);
    }

    [System.Serializable]
    public class LoopScrollPrefabSourceInstance : LoopScrollPrefabSource
    {
        public string prefabName;
        public int poolSize = 5;

        // 由上层（HotfixView 端）注入池接入，避免 Loader 程序集依赖 HotfixView 中的 GameObjectPoolHelper。
        // 未注入时回退到 Resources.Load + Instantiate / Destroy（仅用于编辑器或最小场景）。
        [StaticField]
        public static Action<string, int> OnInitPool;

        [StaticField]
        public static Func<string, GameObject> OnGetFromPool;

        [StaticField]
        public static Action<GameObject> OnReturnToPool;

        private bool inited;

        public virtual GameObject GetObject(int index)
        {
            try
            {
                if (!this.inited)
                {
                    OnInitPool?.Invoke(this.prefabName, this.poolSize);
                    this.inited = true;
                }
                if (OnGetFromPool != null)
                {
                    return OnGetFromPool(this.prefabName);
                }
                GameObject prefab = Resources.Load<GameObject>(this.prefabName);
                return prefab != null ? UnityEngine.Object.Instantiate(prefab) : null;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return null;
            }
        }

        public virtual void ReturnObject(Transform go, bool isDestroy = false)
        {
            try
            {
                if (isDestroy || OnReturnToPool == null)
                {
                    UnityEngine.GameObject.Destroy(go.gameObject);
                    return;
                }
                OnReturnToPool(go.gameObject);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}
