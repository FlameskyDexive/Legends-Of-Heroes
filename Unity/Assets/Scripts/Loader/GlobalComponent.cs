using UnityEngine;

namespace ET
{
    [FriendOf(typeof(GlobalComponent))]
    public static partial class GlobalComponentSystem
    {
        [EntitySystem]
        public static void Awake(this GlobalComponent self)
        {
            self.Global = GameObject.Find("/Global").transform;
            self.Unit = GameObject.Find("/Global/Unit").transform;
            self.UI = GameObject.Find("/Global/UI").transform;
            self.NormalRoot = GameObject.Find("/Global/UI/NormalRoot").transform;
            self.PopUpRoot = GameObject.Find("/Global/UI/PopUpRoot").transform;
            self.FixedRoot = GameObject.Find("/Global/UI/FixedRoot").transform;
            self.OtherRoot = GameObject.Find("/Global/UI/OtherRoot").transform;
            self.PoolRoot =  GameObject.Find("/Global/PoolRoot").transform;
            
            self.GlobalConfig = Resources.Load<GlobalConfig>("GlobalConfig");
        }
    }
    
    [ComponentOf(typeof(Scene))]
    public class GlobalComponent: Entity, IAwake
    {
        public Transform Global;
        public Transform Unit { get; set; }
        public Transform UI;

        public GlobalConfig GlobalConfig { get; set; }
        
        public Transform NormalRoot{ get; set; }
        public Transform PopUpRoot{ get; set; }
        public Transform FixedRoot{ get; set; }
        public Transform PoolRoot{ get; set; }
        public Transform OtherRoot{ get; set; }
    }
}