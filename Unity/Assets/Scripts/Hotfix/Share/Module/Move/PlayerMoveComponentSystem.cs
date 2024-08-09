using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace ET
{
    [EntitySystemOf(typeof(PlayerMoveComponent))]
    [FriendOf(typeof(PlayerMoveComponent))]
    public static partial class PlayerMoveComponentSystem
    {

        [EntitySystem]
        private static void Destroy(this PlayerMoveComponent self)
        {
            
        }

        [EntitySystem]
        private static void Awake(this PlayerMoveComponent self)
        {
            self.IsMoving = false;
        }
        [EntitySystem]
        private static void FixedUpdate(this PlayerMoveComponent self)
        {
            self.UpdateMove();
        }

        public static void UpdateMove(this PlayerMoveComponent self)
        {
            Unit unit = self.GetParent<Unit>();
            if (!self.IsMoving)
                return;
            float speed = unit.GetComponent<NumericComponent>().GetAsFloat(NumericType.Speed);
            speed = speed == 0 ? 3 : speed;
            float3 v3 = unit.Position + unit.Forward * speed / DefineCore.LogicFrame;
            unit.Position = v3;
        }
        
        public static void StartMove(this PlayerMoveComponent self)
        {
            self.IsMoving = true;
        }

        public static void StopMove(this PlayerMoveComponent self)
        {

            self.IsMoving = false;
        }
    }
}