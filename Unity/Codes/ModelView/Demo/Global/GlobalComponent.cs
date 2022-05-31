using UnityEngine;

namespace ET
{
    [ComponentOf(typeof(Scene))]
    public class GlobalComponent: Entity, IAwake
    {
        public static GlobalComponent Instance;

        public Transform Global { get; set; }
        public Transform Unit{ get; set; }
        public Transform UI{ get; set; }
        public Transform NormalRoot{ get; set; }
        public Transform PopUpRoot{ get; set; }
        public Transform FixedRoot{ get; set; }
        public Transform PoolRoot{ get; set; }
        public Transform OtherRoot{ get; set; }
    }
}