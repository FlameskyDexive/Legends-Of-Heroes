using Cinemachine;
using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(BallCameraComponent))]
    [FriendOf(typeof(BallCameraComponent))]
    public static partial class BallCameraComponentSystem
    {
        // 俯视相机离地高度(正交模式下高度不影响成像大小,仅决定俯拍距离/裁剪)
        private const float CameraHeight = 30f;
        // 正交视口半高基准(小球初始视野);随球体积动态放大。球初始直径约 0.72 → 这里给一个能看清小球+周围食物的近视野。
        private const float BaseOrthographicSize = 2.5f;
        // 正交视口随球半径放大的系数(球越大视野越广,agar.io 手感):orthoSize = Base + radius * ZoomPerRadius
        private const float ZoomPerRadius = 2.5f;
        // 平滑速度(指数平滑,帧率无关):位置跟随快一点、缩放慢一点更柔和。
        private const float FollowSmooth = 10f;
        private const float ZoomSmooth = 4f;

        [EntitySystem]
        private static void Awake(this BallCameraComponent self)
        {
            // 球球大作战是平面玩法:关掉客户端"贴地"射线,让球保持服务端 Y=0(否则会被 BallArena 场景的 Map 层几何贴到非0高度,
            // 玩家高度漂移、食物甚至被推到背景下方而看不见),同时省掉每次移动的射线开销。离开球图(Destroy)时还原。
            GameObjectPosConfig.EnableTerrain = false;

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

            // 目标:相机在球正上方垂直俯拍(跟随 XZ,高度固定)
            Vector3 p = ball.transform.position;
            Vector3 targetPos = new Vector3(p.x, p.y + CameraHeight, p.z);

            // 目标正交视野:随球体积动态(球越大视野越广,小球时拉近)。
            // NumericWatcher_Radius_ScaleBall 把 ball.localScale 设为直径(=半径*2),故 radius = localScale.x/2。
            float radius = ball.transform.localScale.x * 0.5f;
            if (radius < 0.01f)
            {
                radius = 0.36f; // 兜底(首帧体型数值还没广播时)
            }
            float targetOrtho = BaseOrthographicSize + radius * ZoomPerRadius;

            // 平滑跟随 + 平滑缩放(指数平滑,与帧率无关:1-e^(-k*dt))
            float dt = Time.deltaTime;
            self.Camera.transform.position = Vector3.Lerp(self.Camera.transform.position, targetPos, 1f - Mathf.Exp(-FollowSmooth * dt));
            self.Camera.orthographicSize = Mathf.Lerp(self.Camera.orthographicSize, targetOrtho, 1f - Mathf.Exp(-ZoomSmooth * dt));
        }

        [EntitySystem]
        private static void Destroy(this BallCameraComponent self)
        {
            // 离开球图,恢复贴地射线(普通 RPG 地图需要)
            GameObjectPosConfig.EnableTerrain = true;

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
