using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ET.Client
{
    /// <summary>
    /// 摇杆组件：
    /// - 支持动态/静态两种模式（dynamic：按下处即为摇杆中心）
    /// - 支持轴向限制（Both / Horizontal / Vertical）
    /// - 支持滑动手势（按下后短时间内抬起且超过阈值距离触发 OnSwipeEvent）
    /// - 提供指向器（arrow）随摇杆方向旋转
    /// </summary>
    public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        private const float DRAG_TIME = 0.15f;

        public int offsetY = -380;
        public float maxRadius = 38;
        public Direction activatedAxis = Direction.Both;

        [SerializeField] private bool dynamic = true;
        [SerializeField] private bool showDirectionArrow = true;
        [SerializeField] private bool hideWhenUndrag = true;

        private CanvasGroup canvasGroup;

        [Space(8)]
        public JoystickEvent OnValueChanged = new JoystickEvent();

        [Space(8)]
        public UnityEvent<Vector2> OnSwipeEvent = new UnityEvent<Vector2>();

        public bool IsDraging => fingerId != int.MinValue;
        public bool ShowDirectionArrow
        {
            get => showDirectionArrow;
            set => showDirectionArrow = value;
        }
        public bool IsDynamic
        {
            get => dynamic;
            set
            {
                if (dynamic != value)
                {
                    dynamic = value;
                    ConfigJoystick();
                }
            }
        }

        private float dragTime;
        private float pointDownTime;

        private void Start()
        {
            this.canvasGroup = this.GetComponentInChildren<CanvasGroup>();
            this.backGroundOriginLocalPostion = (this.backGround.localPosition + new Vector3(0, this.offsetY, 0));
            RestJoystick();
        }

        private void Update()
        {
            if (this.IsDraging && this.dragTime > DRAG_TIME)
            {
                OnValueChanged?.Invoke(knob.localPosition / maxRadius);
            }
        }

        private void OnDisable()
        {
            RestJoystick();
        }

        private void OnValidate()
        {
            ConfigJoystick();
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (eventData.pointerId < -1 || IsDraging)
            {
                return;
            }
            fingerId = eventData.pointerId;
            pointerDownPosition = eventData.position;
            if (dynamic)
            {
                pointerDownPosition[2] = eventData.pressEventCamera?.WorldToScreenPoint(backGround.position).z ?? backGround.position.z;
                backGround.position = eventData.pressEventCamera?.ScreenToWorldPoint(pointerDownPosition) ?? pointerDownPosition;
            }
            OnPointerDown.Invoke(eventData.position);
            pointDownTime = Time.realtimeSinceStartup;
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (fingerId != eventData.pointerId)
            {
                return;
            }
            this.dragTime = Time.realtimeSinceStartup - this.pointDownTime;
            if (this.dragTime > DRAG_TIME && this.hideWhenUndrag && this.canvasGroup != null)
            {
                if (1 - this.canvasGroup.alpha > float.Epsilon)
                {
                    this.canvasGroup.alpha = 1;
                }
            }
            Vector2 direction = eventData.position - (Vector2)pointerDownPosition;
            float radius = Mathf.Clamp(Vector3.Magnitude(direction), 0, maxRadius);
            Vector2 localPosition = new Vector2
            {
                x = (activatedAxis == Direction.Both || activatedAxis == Direction.Horizontal) ? (direction.normalized * radius).x : 0,
                y = (activatedAxis == Direction.Both || activatedAxis == Direction.Vertical) ? (direction.normalized * radius).y : 0,
            };
            knob.localPosition = localPosition;
            if (showDirectionArrow)
            {
                if (!arrow.gameObject.activeInHierarchy)
                {
                    arrow.gameObject.SetActive(true);
                }
                arrow.localEulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.right, localPosition));
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (fingerId != eventData.pointerId)
            {
                return;
            }
            // 短按 + 滑动距离阈值 => 滑动手势
            if (this.dragTime < DRAG_TIME)
            {
                if ((eventData.position - (Vector2)pointerDownPosition).magnitude > 50)
                {
                    OnSwipeEvent.Invoke((eventData.position - (Vector2)pointerDownPosition).normalized);
                }
            }
            RestJoystick();
            OnPointerUp.Invoke(eventData.position);
        }

        private void RestJoystick()
        {
            if (this.IsDraging && this.dragTime > DRAG_TIME)
            {
                OnValueChanged?.Invoke(Vector2.zero);
            }
            backGround.localPosition = backGroundOriginLocalPostion;
            knob.localPosition = Vector3.zero;
            arrow.gameObject.SetActive(false);
            fingerId = int.MinValue;
            this.dragTime = 0;
            this.pointDownTime = 0;
            if (this.hideWhenUndrag && this.canvasGroup != null)
            {
                this.canvasGroup.alpha = 0;
            }
        }

        private void ConfigJoystick()
        {
            if (!dynamic)
            {
                backGroundOriginLocalPostion = backGround.localPosition;
            }
        }

        [HideInInspector] public JoystickEvent OnPointerDown = new JoystickEvent();
        [HideInInspector] public JoystickEvent OnPointerUp = new JoystickEvent();
        [SerializeField, HideInInspector] public Transform knob;
        [SerializeField, HideInInspector] public Transform backGround;
        [SerializeField, HideInInspector] public Transform arrow;

        private Vector3 backGroundOriginLocalPostion;
        private Vector3 pointerDownPosition;
        private int fingerId = int.MinValue;

        [System.Serializable]
        public class JoystickEvent : UnityEvent<Vector2>
        {
        }

        public enum Direction
        {
            Both,
            Horizontal,
            Vertical,
        }
    }
}
