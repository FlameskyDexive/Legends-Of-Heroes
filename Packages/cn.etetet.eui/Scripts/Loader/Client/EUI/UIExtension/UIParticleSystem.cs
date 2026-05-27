using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace ET.Client
{
    /// <summary>
    /// UI 粒子系统：在 UI Canvas 上以 MaskableGraphic 方式渲染 ParticleSystem 粒子。
    /// 用法：挂在带 ParticleSystem 的 GameObject 上，设置 particleTexture 或 particleSprite。
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(ParticleSystem))]
    public class UIParticleSystem : MaskableGraphic
    {
        public Texture particleTexture;
        public Sprite particleSprite;

        private Transform _transform;
        private ParticleSystem _particleSystem;
        private ParticleSystem.Particle[] _particles;
        private readonly UIVertex[] _quad = new UIVertex[4];
        private Vector4 _uv = Vector4.zero;
        private ParticleSystem.TextureSheetAnimationModule _textureSheetAnimation;
        private int _textureSheetAnimationFrames;
        private Vector2 _textureSheedAnimationFrameSize;

        public override Texture mainTexture
        {
            get
            {
                if (particleTexture)
                {
                    return particleTexture;
                }
                if (particleSprite)
                {
                    return particleSprite.texture;
                }
                return null;
            }
        }

        protected bool Initialize()
        {
            if (_transform == null)
            {
                _transform = transform;
            }

            ParticleSystemRenderer particleRenderer = GetComponent<ParticleSystemRenderer>();
            bool setParticleSystemMaterial = false;

            if (_particleSystem == null)
            {
                _particleSystem = GetComponent<ParticleSystem>();
                if (_particleSystem == null)
                {
                    return false;
                }
                if (particleRenderer == null)
                {
                    particleRenderer = _particleSystem.gameObject.AddComponent<ParticleSystemRenderer>();
                }
                Material currentMaterial = particleRenderer.sharedMaterial;
                if (currentMaterial && currentMaterial.HasProperty("_MainTex"))
                {
                    particleTexture = currentMaterial.mainTexture;
                }
                var mainModule = _particleSystem.main;
                mainModule.scalingMode = ParticleSystemScalingMode.Local;
                _particles = null;
                setParticleSystemMaterial = true;
            }
            else
            {
                if (Application.isPlaying)
                {
                    setParticleSystemMaterial = (particleRenderer.material == null);
                }
#if UNITY_EDITOR
                else
                {
                    setParticleSystemMaterial = (particleRenderer.sharedMaterial == null);
                }
#endif
            }

            if (setParticleSystemMaterial)
            {
                Material mat = new Material(Shader.Find("UI/Particles/Hidden"));
                if (Application.isPlaying)
                {
                    particleRenderer.material = mat;
                }
#if UNITY_EDITOR
                else
                {
                    mat.hideFlags = HideFlags.DontSave;
                    particleRenderer.sharedMaterial = mat;
                }
#endif
            }

            if (_particles == null)
            {
                _particles = new ParticleSystem.Particle[_particleSystem.main.maxParticles];
            }

            if (particleTexture)
            {
                _uv = new Vector4(0, 0, 1, 1);
            }
            else if (particleSprite)
            {
                _uv = DataUtility.GetOuterUV(particleSprite);
            }

            _textureSheetAnimation = _particleSystem.textureSheetAnimation;
            _textureSheetAnimationFrames = 0;
            _textureSheedAnimationFrameSize = Vector2.zero;
            if (_textureSheetAnimation.enabled)
            {
                _textureSheetAnimationFrames = _textureSheetAnimation.numTilesX * _textureSheetAnimation.numTilesY;
                _textureSheedAnimationFrameSize = new Vector2(1f / _textureSheetAnimation.numTilesX, 1f / _textureSheetAnimation.numTilesY);
            }

            return true;
        }

        protected override void Awake()
        {
            base.Awake();
            if (!Initialize())
            {
                enabled = false;
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (!Initialize())
                {
                    return;
                }
            }
#endif
            vh.Clear();

            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            int count = _particleSystem.GetParticles(_particles);
            for (int i = 0; i < count; ++i)
            {
                ParticleSystem.Particle particle = _particles[i];

                Vector2 position = _particleSystem.main.simulationSpace == ParticleSystemSimulationSpace.Local
                    ? particle.position
                    : (Vector2)_transform.InverseTransformPoint(particle.position);
                float rotation = -particle.rotation * Mathf.Deg2Rad;
                float rotation90 = rotation + Mathf.PI / 2;
                Color32 color = particle.GetCurrentColor(_particleSystem);
                float size = particle.GetCurrentSize(_particleSystem) * 0.5f;

                if (_particleSystem.main.scalingMode == ParticleSystemScalingMode.Shape)
                {
                    position /= canvas.scaleFactor;
                }

                Vector4 particleUV = _uv;
                if (_textureSheetAnimation.enabled)
                {
                    float frameProgress = 1 - (particle.remainingLifetime / particle.startLifetime);
                    frameProgress = Mathf.Repeat(frameProgress * _textureSheetAnimation.cycleCount, 1);
                    int frame = 0;
                    switch (_textureSheetAnimation.animation)
                    {
                        case ParticleSystemAnimationType.WholeSheet:
                            frame = Mathf.FloorToInt(frameProgress * _textureSheetAnimationFrames);
                            break;
                        case ParticleSystemAnimationType.SingleRow:
                            frame = Mathf.FloorToInt(frameProgress * _textureSheetAnimation.numTilesX);
                            int row = _textureSheetAnimation.rowIndex;
                            frame += row * _textureSheetAnimation.numTilesX;
                            break;
                    }
                    frame %= _textureSheetAnimationFrames;

                    particleUV.x = (frame % _textureSheetAnimation.numTilesX) * _textureSheedAnimationFrameSize.x;
                    particleUV.y = Mathf.FloorToInt(frame / _textureSheetAnimation.numTilesX) * _textureSheedAnimationFrameSize.y;
                    particleUV.z = particleUV.x + _textureSheedAnimationFrameSize.x;
                    particleUV.w = particleUV.y + _textureSheedAnimationFrameSize.y;
                }

                _quad[0] = UIVertex.simpleVert;
                _quad[0].color = color;
                _quad[0].uv0 = new Vector2(particleUV.x, particleUV.y);

                _quad[1] = UIVertex.simpleVert;
                _quad[1].color = color;
                _quad[1].uv0 = new Vector2(particleUV.x, particleUV.w);

                _quad[2] = UIVertex.simpleVert;
                _quad[2].color = color;
                _quad[2].uv0 = new Vector2(particleUV.z, particleUV.w);

                _quad[3] = UIVertex.simpleVert;
                _quad[3].color = color;
                _quad[3].uv0 = new Vector2(particleUV.z, particleUV.y);

                if (rotation == 0)
                {
                    Vector2 corner1 = new Vector2(position.x - size, position.y - size);
                    Vector2 corner2 = new Vector2(position.x + size, position.y + size);
                    _quad[0].position = new Vector2(corner1.x, corner1.y);
                    _quad[1].position = new Vector2(corner1.x, corner2.y);
                    _quad[2].position = new Vector2(corner2.x, corner2.y);
                    _quad[3].position = new Vector2(corner2.x, corner1.y);
                }
                else
                {
                    Vector2 right = new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation)) * size;
                    Vector2 up = new Vector2(Mathf.Cos(rotation90), Mathf.Sin(rotation90)) * size;
                    _quad[0].position = position - right - up;
                    _quad[1].position = position - right + up;
                    _quad[2].position = position + right + up;
                    _quad[3].position = position + right - up;
                }

                vh.AddUIVertexQuad(_quad);
            }
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                _particleSystem.Simulate(Time.unscaledDeltaTime, false, false);
                SetAllDirty();
            }
        }

#if UNITY_EDITOR
        private void LateUpdate()
        {
            if (!Application.isPlaying)
            {
                SetAllDirty();
            }
        }
#endif
    }
}
