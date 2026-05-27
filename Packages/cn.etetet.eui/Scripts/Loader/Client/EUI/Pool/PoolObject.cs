using UnityEngine;

namespace ET.Client
{
    [DisallowMultipleComponent]
    [AddComponentMenu("")]
    public class PoolObject : MonoBehaviour
    {
        public string poolName;
        public bool isPooled;
    }
}
