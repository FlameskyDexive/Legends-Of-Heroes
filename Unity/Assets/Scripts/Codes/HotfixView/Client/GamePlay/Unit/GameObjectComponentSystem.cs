using System;
using UnityEngine;

namespace ET.Client
{
    public static class GameObjectComponentSystem
    {
        [ObjectSystem]
        public class DestroySystem: DestroySystem<GameObjectComponent>
        {
            protected override void Destroy(GameObjectComponent self)
            {
                UnityEngine.Object.Destroy(self.GameObject);
            }
        }
        [ObjectSystem]
        public class UpdateSystem: UpdateSystem<GameObjectComponent>
        {
            protected override void Update(GameObjectComponent self)
            {
                self.Update();
            }
        }


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