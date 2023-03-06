using UnityEngine;

namespace ET.Client
{
    [ComponentOf(typeof(Unit))]
    public class GameObjectComponent: Entity, IAwake, IUpdate, IDestroy
    {
        public GameObject GameObject { get; set; }
    }
}