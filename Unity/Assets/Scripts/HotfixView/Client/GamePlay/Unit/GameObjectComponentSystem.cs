using System;
using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(GameObjectComponent))]
    public static partial class GameObjectComponentSystem
    {
        [EntitySystem]
        private static void Destroy(this GameObjectComponent self)
        {
            UnityEngine.Object.Destroy(self.GameObject);
        }
        
        [EntitySystem]
        private static void Awake(this GameObjectComponent self)
        {

        }


        public static void Init(this GameObjectComponent self, GameObject go)
        {
            self.GameObject = go;
            self.SyncUnitTransform();
        }
        public static void SyncUnitTransform(this GameObjectComponent self)
        {
            if (self.GameObject == null)
                return;
            Unit unit = self.GetParent<Unit>();
            if (unit.Type() == EUnitType.Player)
            {
                Transform dirTrans = self.GameObject.transform.Find("RootDir");
                if (dirTrans != null)
                {
                    dirTrans.rotation = unit.Rotation;
                    return;
                }
            }
            Transform transform = self.GameObject.transform;
            transform.rotation = unit.Rotation;
            // self.GameObject.transform.position = unit.Position;
        }

        public static void RefreshScale(this GameObjectComponent self)
        {
            if (self.GameObject == null)
                return;

            Unit unit = self.GetParent<Unit>();
            int hp = unit.GetComponent<NumericComponent>().GetAsInt(NumericType.Hp);
            if (hp > 10)
                self.GameObject.transform.localScale = Vector3.one * hp / 50f;
        }

        [EntitySystem]
        public static void Update(this GameObjectComponent self)
        {
            if (self.GameObject != null)
            {
                Unit unit = self.GetParent<Unit>();
                self.GameObject.transform.position = Vector3.Lerp(self.GameObject.transform.position, unit.Position, Time.deltaTime * 5f);
            }
        }
    }
}