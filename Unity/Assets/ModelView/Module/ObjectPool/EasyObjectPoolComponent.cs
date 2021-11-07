using System;
using UnityEngine;
using System.Collections.Generic;

namespace ET
{
    public class EasyObjectPoolComponent : Entity
    {
        public Dictionary<string, Pool> poolDict = new Dictionary<string, Pool>();
    }
}