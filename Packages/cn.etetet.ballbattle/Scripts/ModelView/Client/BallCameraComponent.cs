using Cinemachine;
using UnityEngine;

namespace ET.Client
{
    // 球球大作战:本地玩家专属俯视正交相机。挂在本地玩家 Unit 上。
    // 复用主相机(Camera.main):暂时禁用其 CinemachineBrain(停止 3D 透视跟随驱动),
    // 改为正交 + 顶视(绕 X 轴 90° 垂直俯拍 XZ 平面),每帧 LateUpdate 跟随本地球;Destroy 时还原。
    [ComponentOf(typeof(Unit))]
    public class BallCameraComponent : Entity, IAwake, ILateUpdate, IDestroy
    {
        public Camera Camera;
        public CinemachineBrain Brain;

        // 还原用的原始相机状态
        public bool OrigBrainEnabled;
        public bool OrigOrthographic;
        public float OrigOrthographicSize;
        public Vector3 OrigPosition;
        public Quaternion OrigRotation;
    }
}
