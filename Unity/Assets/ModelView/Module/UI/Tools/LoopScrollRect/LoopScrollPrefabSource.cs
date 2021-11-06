using System;
using UnityEngine;
using System.Collections;
using ET;


namespace UnityEngine.UI
{
    [System.Serializable]
    public class LoopScrollPrefabSource 
    {
        public string prefabName;
        public int poolSize = 5;

        private bool inited = false;
        public virtual GameObject GetObject()
        {
            try
            {
                if(!inited)
                {
                    Game.Scene.GetComponent<EasyObjectPoolComponent>().InitPool(prefabName, poolSize);
                    inited = true;
                }
                return Game.Scene.GetComponent<EasyObjectPoolComponent>().GetObjectFromPool(prefabName);
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }
        
        public virtual void ReturnObject(Transform go , bool isDestroy = false)
        {
            try
            {
                if (isDestroy)
                {
                    UnityEngine.GameObject.Destroy(go.gameObject);
                }
                else
                {
                    Game.Scene.GetComponent<EasyObjectPoolComponent>().ReturnObjectToPool(go.gameObject);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            
        }
    }
}
