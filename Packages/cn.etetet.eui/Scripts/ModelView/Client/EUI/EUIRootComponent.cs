using UnityEngine;

namespace ET.Client
{
    [ComponentOf(typeof(Scene))]
    public class EUIRootComponent : Entity, IAwake, IDestroy
    {
        public Transform Root;
        public Transform NormalRoot;
        public Transform FixedRoot;
        public Transform PopUpRoot;
        public Transform OtherRoot;
    }
}
