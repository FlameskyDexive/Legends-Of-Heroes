using UnityEngine;

namespace ET
{
    [FriendOf(typeof(GlobalComponent))]
    public static partial class GlobalComponentSystem
    {
        [EntitySystem]
        public static void Awake(this GlobalComponent self)
        {
            // self.Global = GameObject.Find("/Global").transform;
            // self.Unit = GameObject.Find("/Global/Unit").transform;
            // self.UI = GameObject.Find("/Global/UI").transform;
            // self.GlobalConfig = Resources.Load<GlobalConfig>("GlobalConfig");

            GlobalComponent.Instance = self;

            self.Global = GameObject.Find("/Global").transform;
            self.Unit = GameObject.Find("/Global/UnitRoot").transform;
            // self.UI = GameObject.Find("/Global/UI").transform;
            self.UICamera = GameObject.Find("/Global/UICamera").GetComponent<Camera>();
            self.UIRoot = GameObject.Find("/Global/UIRoot").transform;
            self.NormalRoot = GameObject.Find("Global/UIRoot/NormalRoot").transform;
            self.PopUpRoot = GameObject.Find("Global/UIRoot/PopUpRoot").transform;
            self.FixedRoot = GameObject.Find("Global/UIRoot/FixedRoot").transform;
            self.OtherRoot = GameObject.Find("Global/UIRoot/OtherRoot").transform;
        }
    }
    
    [ComponentOf(typeof(Scene))]
    public class GlobalComponent: Entity, IAwake
    {
        // public Transform Global;
        // public Transform Unit { get; set; }
        // public Transform UI;
        //
        // public GlobalConfig GlobalConfig { get; set; }

        [StaticField]
        public static GlobalComponent Instance;

        public Transform Global;
        public Transform Unit { get; set; }
        public Transform UI;

        public Camera UICamera { get; set; }
        public Transform UIRoot { get; set; }
        public Transform NormalRoot { get; set; }
        public Transform PopUpRoot { get; set; }
        public Transform FixedRoot { get; set; }
        public Transform PoolRoot { get; set; }
        public Transform OtherRoot { get; set; }
    }
}