using System;
using UnityEngine;
using System.Collections.Generic;

namespace ET
{

    public enum PoolGameObjectResType
    {
        None,
        UI_LoopItem,
    }
    
    
    public class EasyObjectPoolComponent : Entity
    {
        //obj pool
        public Dictionary<string, Pool> poolDict = new Dictionary<string, Pool>();


    }
}