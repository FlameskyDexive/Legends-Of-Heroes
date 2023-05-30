using UnityEngine;

namespace ET.Client
{
    [ComponentOf(typeof(Scene))]
    public class NetworkCheckComponent : Entity, IAwake, IDestroy
    {
        public int interval = 1000;

        public long timer;
    }
}