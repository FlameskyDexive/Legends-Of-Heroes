using System;
using Unity.Mathematics;
using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(CameraComponent))]
    public static partial class CameraComponentSystem
    {
        [EntitySystem]
        private static void Destroy(this CameraComponent self)
        {
            
        }
        
        [EntitySystem]
        private static void Awake(this CameraComponent self)
        {
            self.MainCamera = Camera.main;
        }

        [EntitySystem]
        private static void LateUpdate(this CameraComponent self)
        {

            // 摄像机每帧更新位置
            self.UpdatePosition();
        }


        private static void UpdatePosition(this CameraComponent self)
        {
            Unit unit = UnitHelper.GetMyUnitFromCurrentScene(self.Root().CurrentScene());
            if (unit != null)
            {
                self.MainCamera.transform.position = math.lerp(self.MainCamera.transform.position, unit.Position + new float3(0, 10, 0), Time.deltaTime * 8f);
            }
        }
    }
}