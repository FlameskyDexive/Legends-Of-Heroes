using UnityEngine;

namespace ET.Client
{
    [ComponentOf(typeof(Scene))]
    public class CameraComponent : Entity, IAwake, ILateUpdate, IDestroy
    {
        // 战斗摄像机
        public Camera mainCamera;

        private EntityRef<Unit> unit;

        public Unit Unit
        {
            get
            {
                return this.unit;
            }
            set
            {
                this.unit = value;
            }
        }

        public Camera MainCamera
        {
            get
            {
                return this.mainCamera;
            }
            set
            {
                this.mainCamera = value;
            }
        }
    }
}