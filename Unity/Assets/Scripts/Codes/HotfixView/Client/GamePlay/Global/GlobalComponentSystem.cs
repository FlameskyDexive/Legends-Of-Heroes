using UnityEngine;

namespace ET.Client
{
    [ObjectSystem]
    public class GlobalComponentAwakeSystem: AwakeSystem<GlobalComponent>
    {
        protected override void Awake(GlobalComponent self)
        {
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
}