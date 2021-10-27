using UnityEngine;

namespace ET
{
    public class GlobalComponentAwakeSystem: AwakeSystem<GlobalComponent>
    {
        public override void Awake(GlobalComponent self)
        {
            GlobalComponent.Instance = self;
            
            self.Global = GameObject.Find("/Global").transform;
            self.Unit = GameObject.Find("/Global/Unit").transform;
            self.UI = GameObject.Find("/Global/UI").transform;
            self.NormalRoot = GameObject.Find("Global/UI/Hidden").transform;
            self.PopUpRoot = GameObject.Find("Global/UI/Low").transform;
            self.FixedRoot = GameObject.Find("Global/UI/Mid").transform;
            self.OtherRoot = GameObject.Find("Global/UI/High").transform;
            self.PoolRoot =  GameObject.Find("Global/PoolRoot").transform;
        }
    }
}