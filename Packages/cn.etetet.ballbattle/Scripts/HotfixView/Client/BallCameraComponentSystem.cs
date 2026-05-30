using Cinemachine;
using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(BallCameraComponent))]
    [FriendOf(typeof(BallCameraComponent))]
    public static partial class BallCameraComponentSystem
    {
        // 俯视相机离地高度(正交模式下高度不影响成像大小,仅决定俯拍距离/裁剪);可按手感调
        private const float CameraHeight = 30f;
        // 正交视口半高基准(世界单位):小球初始视野,越小越拉近;随球体积动态放大
        private const float BaseOrthographicSize = 6f;
        // 正交视口随球半径放大的系数(球越大视野越广,agar.io 手感):orthoSize = Base + radius * ZoomPerRadius
        private const float ZoomPerRadius = 1.2f;

        [EntitySystem]
        private static void Awake(this BallCameraComponent self)
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                Log.Error("[球球大作战] 找不到主相机(Camera.main),俯视相机装配失败");
                return;
            }
            self.Camera = cam;

            // 记录原始状态,离开地图时还原
            self.OrigOrthographic = cam.orthographic;
            self.OrigOrthographicSize = cam.orthographicSize;
            self.OrigPosition = cam.transform.position;
            self.OrigRotation = cam.transform.rotation;

            // 禁用 Cinemachine Brain,避免它继续驱动相机覆盖我们的俯视设置
            self.Brain = cam.GetComponent<CinemachineBrain>();
            if (self.Brain != null)
            {
                self.OrigBrainEnabled = self.Brain.enabled;
                self.Brain.enabled = false;
            }

            // 切正交 + 顶视(绕 X 轴 90° 垂直向下俯拍 XZ 平面)
            cam.orthographic = true;
            cam.orthographicSize = BaseOrthographicSize;
            cam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }

        [EntitySystem]
        private static void LateUpdate(this BallCameraComponent self)
        {
            if (self.Camera == null)
            {
                return;
            }
            GameObjectComponent goComponent = self.GetParent<Unit>().GetComponent<GameObjectComponent>();
            GameObject ball = goComponent?.GameObject;
            if (ball == null)
            {
                return;
            }
            // 俯视:相机在球正上方垂直俯拍(跟随 XZ,高度固定)
            Vector3 p = ball.transform.position;
            self.Camera.transform.position = new Vector3(p.x, p.y + CameraHeight, p.z);

            // 正交视野随球体积动态:球越大视野越广,小球时拉近(初始不会"拉那么高")。
            // NumericWatcher_Radius_ScaleBall 把 ball.localScale 设为直径(=半径*2),故 radius = localScale.x/2。
            float radius = ball.transform.localScale.x * 0.5f;
            if (radius < 0.01f)
            {
                radius = 0.5f; // 兜底(首帧体型数值还没广播时)
            }
            self.Camera.orthographicSize = BaseOrthographicSize + radius * ZoomPerRadius;
        }

        [EntitySystem]
        private static void Destroy(this BallCameraComponent self)
        {
            Camera cam = self.Camera;
            if (cam != null)
            {
                cam.orthographic = self.OrigOrthographic;
                cam.orthographicSize = self.OrigOrthographicSize;
                cam.transform.position = self.OrigPosition;
                cam.transform.rotation = self.OrigRotation;
            }
            if (self.Brain != null)
            {
                self.Brain.enabled = self.OrigBrainEnabled;
            }
            self.Camera = null;
            self.Brain = null;
        }
    }
}
