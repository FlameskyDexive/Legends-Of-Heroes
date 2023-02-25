using ET.Client;
using System.Collections;
using UnityEngine;

namespace ET
{
    public static class EUIHudItemHelper
    {
        public static T CreateHudEntity<T>() where T : Entity, IAwake
        {
            var ui = Game.BattleScene?.GetComponent<UIComponent>()?.GetDlgLogic<DlgHUD>();
            if (ui != null)
            {
                T hud = ui.AddChild<T>(true);

                ui.AddHudEntity(hud);

                return hud;
            }
            else
            {
                Log.Error("#程序请检查 # 当前没有打开hud界面");
            }

            return default;
        }

        public static void RemoveHudEntity(long id)
        {
            Game.BattleScene?.GetComponent<UIComponent>()?.GetDlgLogic<DlgHUD>()?.RemoveHudEntity(id);
        }

        public static void AddHud(Transform transform)
        {
            Game.BattleScene?.GetComponent<UIComponent>()?.GetDlgLogic<DlgHUD>()?.AddHud(transform);
        }

        public static RectTransform GetRoot()
        {
            return Game.BattleScene?.GetComponent<UIComponent>()?.GetDlgLogic<DlgHUD>()?.GetRoot();
        }

        #region ui跟随相关
        private static Vector2 WorldPosToScreenPos(Vector3 worldPos)
        {
            //待优化
            var camera = Camera.main;
            if (camera == null)
                return default;

            var scale = 1f;
            var canvas = EUIRootHelper.GetTargetRoot(UIWindowType.Normal)?.GetComponent<Canvas>();
            if (canvas != null)
                scale = canvas.scaleFactor;

            Vector3 viewportPoint = camera.WorldToViewportPoint(worldPos);

            //TODO : 当投射到视口坐标的z轴为负时，代表在摄像机背面，这时候x,y的值就反了
            if (viewportPoint.z < 0)
            {
                viewportPoint.x = -viewportPoint.x;
                viewportPoint.y = -viewportPoint.y;
            }

            Vector3 screenPoint = new Vector3(viewportPoint.x * Screen.width,
                                                viewportPoint.y * Screen.height,
                                                0);
            screenPoint.x -= Screen.width * .5f;
            screenPoint.y -= Screen.height * .5f;

            var outPos = new Vector2(screenPoint.x / scale, screenPoint.y / scale);

            return outPos;
        }

        public static bool IsOutOfClampRect(RectTransform content, Vector3 worldPos)
        {
            if (content == null)
                return false;

            var screenPos = WorldPosToScreenPos(worldPos);

            var halfScreenSize = new Vector2(content.rect.width * .5f, content.rect.height * .5f);
            return !MathHelper.IsContainsPoint(-halfScreenSize.x, -halfScreenSize.y, halfScreenSize.x, halfScreenSize.y, screenPos);
        }

        public static void UpdatePostion(RectTransform hudTransform, Vector3 worldPos)
        {
            if (hudTransform == null)
                return;

            hudTransform.anchoredPosition = WorldPosToScreenPos(worldPos);
        }

        public static void UpdateRotation(RectTransform transform, Vector3 forward)
        {
            if (transform == null)
                return;

            transform.rotation = Quaternion.AngleAxis(-Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg, Vector3.forward);
        }

        public static void UpdatePositionClampInsideScreen(RectTransform content, RectTransform hudTransform, Vector3 worldPos)
        {
            if (content == null)
                return;

            if (hudTransform == null)
                return;

            var screenPos = WorldPosToScreenPos(worldPos);

            var halfScreenSize = new Vector2(content.rect.width * .5f, content.rect.height * .5f);
            if (!MathHelper.IsContainsPoint(-halfScreenSize.x, -halfScreenSize.y, halfScreenSize.x, halfScreenSize.y, screenPos))
            {
                var delta = screenPos/* - Vector2.zero*/;
                float absX = Mathf.Abs(delta.x);
                float absY = Mathf.Abs(delta.y);
                //假定x是固定
                screenPos = GetAnglePointByX(halfScreenSize, delta, absX, absY);
                if (Mathf.Abs(screenPos.y) > halfScreenSize.y)
                {
                    //假定y是固定
                    screenPos = GetAnglePointByY(halfScreenSize, delta, absX, absY);
                }

                //icon本身大小偏移
                screenPos -= delta.normalized * hudTransform.sizeDelta.x;
            }

            hudTransform.anchoredPosition = screenPos;
        }

        private static Vector2 GetAnglePointByX(Vector2 halfScreenSize, Vector2 vec2, float absX, float absY)
        {
            Vector2 pos = Vector2.zero;
            pos.x = (vec2.x < 0 ? -halfScreenSize.x : halfScreenSize.x);
            pos.y = (absY * halfScreenSize.x) / absX * (vec2.y < 0 ? -1 : 1);
            return pos;
        }

        private static Vector2 GetAnglePointByY(Vector2 halfScreenSize, Vector2 vec2, float absX, float absY)
        {
            Vector2 pos = Vector2.zero;
            pos.y = (vec2.y < 0 ? -halfScreenSize.y : halfScreenSize.y);
            pos.x = (absX * halfScreenSize.y) / absY * (vec2.x < 0 ? -1 : 1);
            return pos;
        }

        #endregion
    }
}