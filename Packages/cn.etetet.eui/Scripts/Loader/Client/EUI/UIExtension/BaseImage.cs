using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ET.Client
{
    /// <summary>
    /// 自绘 Image 的基类，子类只需重写 OnPopulateMesh 即可绘制自定义形状（如 CircleImage）。
    /// </summary>
    public class BaseImage : MaskableGraphic, ISerializationCallbackReceiver, ILayoutElement, ICanvasRaycastFilter
    {
        [FormerlySerializedAs("m_Frame")]
        [SerializeField]
        private Sprite m_Sprite;

        public Sprite sprite
        {
            get => m_Sprite;
            set
            {
                if (SetPropertyUtilityExt.SetClass(ref m_Sprite, value))
                {
                    SetAllDirty();
                }
            }
        }

        [NonSerialized]
        private Sprite m_OverrideSprite;

        public Sprite overrideSprite
        {
            get => m_OverrideSprite == null ? sprite : m_OverrideSprite;
            set
            {
                if (SetPropertyUtilityExt.SetClass(ref m_OverrideSprite, value))
                {
                    SetAllDirty();
                }
            }
        }

        public override Texture mainTexture =>
            overrideSprite == null ? s_WhiteTexture : overrideSprite.texture;

        public float pixelsPerUnit
        {
            get
            {
                float spritePixelsPerUnit = 100;
                if (sprite)
                {
                    spritePixelsPerUnit = sprite.pixelsPerUnit;
                }
                float referencePixelsPerUnit = 100;
                if (canvas)
                {
                    referencePixelsPerUnit = canvas.referencePixelsPerUnit;
                }
                return spritePixelsPerUnit / referencePixelsPerUnit;
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
        }

        #region ISerializationCallbackReceiver
        public void OnAfterDeserialize() { }
        public void OnBeforeSerialize() { }
        #endregion

        #region ILayoutElement
        public virtual void CalculateLayoutInputHorizontal() { }
        public virtual void CalculateLayoutInputVertical() { }
        public virtual float minWidth => 0;
        public virtual float preferredWidth => overrideSprite == null ? 0 : overrideSprite.rect.size.x / pixelsPerUnit;
        public virtual float flexibleWidth => -1;
        public virtual float minHeight => 0;
        public virtual float preferredHeight => overrideSprite == null ? 0 : overrideSprite.rect.size.y / pixelsPerUnit;
        public virtual float flexibleHeight => -1;
        public virtual int layoutPriority => 0;
        #endregion

        #region ICanvasRaycastFilter
        public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            return true;
        }
        #endregion
    }
}
