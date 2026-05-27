using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    /// <summary>
    /// 红点显示层 MonoBehaviour，挂在需要显示红点的目标 UI 上。
    /// </summary>
    public class RedDotMonoView : MonoBehaviour
    {
        [HideInInspector]
        public bool isRedDotActive;

        private GameObject redDotGameObject;
        private Text redDotCountLabel;

        public Vector3 RedDotScale = Vector3.one;
        public Vector2 PositionOffset = Vector2.zero;

        private void Awake()
        {
            this.isRedDotActive = false;
        }

        public void Show(GameObject redDot)
        {
            if (redDot == null)
            {
                return;
            }
            this.isRedDotActive = true;
            this.redDotGameObject = redDot;
            redDot.transform.SetParent(this.transform, false);
            redDot.transform.localScale = this.RedDotScale;
            var rt = redDot.transform as RectTransform;
            if (rt != null)
            {
                rt.anchoredPosition = this.PositionOffset;
            }
            this.redDotCountLabel = redDot.GetComponentInChildren<Text>();
            redDot.SetActive(true);
        }

        public void RefreshRedDotCount(int count)
        {
            if (this.redDotGameObject == null)
            {
                return;
            }
            this.redDotGameObject.transform.localScale = this.RedDotScale;
            if (this.redDotCountLabel != null)
            {
                this.redDotCountLabel.text = count <= 0 ? string.Empty : count.ToString();
            }
        }

        public GameObject Recovery()
        {
            if (this.redDotCountLabel != null)
            {
                this.redDotCountLabel.text = string.Empty;
            }
            this.isRedDotActive = false;
            this.redDotCountLabel = null;
            this.redDotGameObject?.SetActive(false);
            var go = this.redDotGameObject;
            this.redDotGameObject = null;
            return go;
        }
    }
}
