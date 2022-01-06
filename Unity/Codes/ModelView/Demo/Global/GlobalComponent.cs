using UnityEngine;

namespace ET
{
    public class GlobalComponent: Entity, IAwake
    {
        public static GlobalComponent Instance;
        
        public Transform Global;
        public Transform Unit;
        public Transform UI;
        public Transform NormalRoot;
        public Transform PopUpRoot;
        public Transform FixedRoot;
        public Transform PoolRoot;
        public Transform OtherRoot;
    }
}