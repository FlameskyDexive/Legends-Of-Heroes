using NLog.Fluent;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace UnityEngine.UI
{
    public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        private const float DRAG_TIME = 0.15f;

        public int offsetY = -380;
        public float maxRadius = 38; //摇杆移动最大半径
        public Direction activatedAxis = Direction.Both; //选择激活的轴向
        [SerializeField] bool dynamic = true; // 是否为动态摇杆
        [SerializeField] bool showDirectionArrow = true; // 是否展示指向器
        [SerializeField]
        bool hideWhenUndrag = true; // 摇杆不拖拽的时候隐藏

        private CanvasGroup canvasGroup;

        [Space(8)]
        public JoystickEvent OnValueChanged = new JoystickEvent(); //事件 ： 摇杆被 拖拽时
        [Space(8)]
        public UnityEvent<Vector2> OnSwipeEvent = new UnityEvent<Vector2>(); //事件 ： 非触发摇杆滑动时

        #region Property
        public bool IsDraging { get { return fingerId != int.MinValue; } } //摇杆拖拽状态
        public bool ShowDirectionArrow { get => showDirectionArrow; set => showDirectionArrow = value; }  // 是否展示指向器
        public bool IsDynamic //运行时代码配置摇杆是否为动态摇杆
        {
            set
            {
                if (dynamic != value)
                {
                    dynamic = value;
                    ConfigJoystick();
                }
            }
            get => dynamic;
        }

        private float dragTime = 0;
        private float pointDownTime = 0;
        #endregion

        #region MonoBehaviour functions
        void Start()
        {
            this.canvasGroup = this.GetComponentInChildren<CanvasGroup>();
            this.backGroundOriginLocalPostion = (this.backGround.localPosition + new Vector3(0, this.offsetY, 0));
            this.RestJoystick();
        }

        void Update()
        {
            if (this.IsDraging && this.dragTime > DRAG_TIME)
                OnValueChanged?.Invoke(knob.localPosition / maxRadius); //fixedupdate 为物理更新，摇杆操作放在常规 update 就好
        }
        void OnDisable()
        {
            RestJoystick(); //意外被 Disable 各单位需要被重置
        }
        void OnValidate() => ConfigJoystick(); //Inspector 发生改变，各单位需要重新配置，编辑器有效
        #endregion

        #region The implement of pointer event Interface
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (eventData.pointerId < -1 || IsDraging)
                return; //适配 Touch：只响应一个Touch；适配鼠标：只响应左键
            fingerId = eventData.pointerId;
            pointerDownPosition = eventData.position;
            if (dynamic)
            {
                pointerDownPosition[2] = eventData.pressEventCamera?.WorldToScreenPoint(backGround.position).z ?? backGround.position.z;
                backGround.position = eventData.pressEventCamera?.ScreenToWorldPoint(pointerDownPosition) ?? pointerDownPosition; ;
            }
            OnPointerDown.Invoke(eventData.position);
            pointDownTime = Time.realtimeSinceStartup;
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (fingerId != eventData.pointerId)
                return;
            this.dragTime = Time.realtimeSinceStartup - this.pointDownTime;
            if (this.dragTime > DRAG_TIME && this.hideWhenUndrag && this.canvasGroup != null)
            {
                if ( 1 - this.canvasGroup.alpha > float.Epsilon)
                    this.canvasGroup.alpha = 1;
            }
            Vector2 direction = eventData.position - (Vector2)pointerDownPosition; //得到BackGround 指向 Handle 的向量
            float radius = Mathf.Clamp(Vector3.Magnitude(direction), 0, maxRadius); //获取并锁定向量的长度 以控制 Handle 半径
            Vector2 localPosition = new Vector2()
            {
                x = (activatedAxis == Direction.Both || activatedAxis == Direction.Horizontal) ? (direction.normalized * radius).x : 0, //确认是否激活水平轴向
                y = (activatedAxis == Direction.Both || activatedAxis == Direction.Vertical) ? (direction.normalized * radius).y : 0       //确认是否激活垂直轴向，激活就搞事情
            };
            knob.localPosition = localPosition;      //更新 Handle 位置
            if (showDirectionArrow)
            {
                if (!arrow.gameObject.activeInHierarchy) arrow.gameObject.SetActive(true);
                arrow.localEulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.right, localPosition));
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (fingerId != eventData.pointerId)
                return;//正确的手指抬起时才会重置摇杆；
            //处理滑动技能
            if (this.dragTime < DRAG_TIME)
            {
                if ((eventData.position - (Vector2)pointerDownPosition).magnitude > 50)
                {
                    Debug.Log($"Swipe screen");
                    OnSwipeEvent.Invoke((eventData.position - (Vector2)pointerDownPosition).normalized);
                }
            }
            RestJoystick();
            OnPointerUp.Invoke(eventData.position);
        }
        #endregion

        #region Assistant functions / fields / structures
        void RestJoystick()  //重置摇杆数据
        {
            if (this.IsDraging && this.dragTime > DRAG_TIME)
                OnValueChanged?.Invoke(Vector2.zero);
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

        void ConfigJoystick() //配置动态/静态摇杆
        {
            if (!dynamic) backGroundOriginLocalPostion = backGround.localPosition;
            //GetComponent<Image>().raycastTarget = dynamic;
            // handle.GetComponent<Image>().raycastTarget = !dynamic;
        }

        [HideInInspector] public JoystickEvent OnPointerDown = new JoystickEvent(); // 事件： 摇杆被按下时
        [HideInInspector] public JoystickEvent OnPointerUp = new JoystickEvent(); //事件 ： 摇杆上抬起时
        [SerializeField, HideInInspector] public Transform knob; //摇杆
        [SerializeField, HideInInspector] public Transform backGround; //背景
        [SerializeField, HideInInspector] public Transform arrow; //指向器
        private Vector3 backGroundOriginLocalPostion, pointerDownPosition;
        private int fingerId = int.MinValue; //当前触发摇杆的 pointerId ，预设一个永远无法企及的值
        [System.Serializable] public class JoystickEvent : UnityEvent<Vector2> { }
        public enum Direction
        {
            Both,
            Horizontal,
            Vertical
        }
        #endregion
    }
}
