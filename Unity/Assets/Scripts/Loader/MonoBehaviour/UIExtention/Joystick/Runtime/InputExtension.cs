using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#region Fake script / 示例代码
namespace Example
{
    using static InputExtension; //使用静态 using 可以直接取用类中成员，本例使用 IsMouseRaycastUI
    class Foo : MonoBehaviour 
    {
        bool canmove;
        int button = 0;//鼠标左键 
        private void Update()
        {
            if (Input.GetMouseButton(button) &&! IsMouseRaycastUI()) 
            {
                canmove = true;
            }
            else if(Input.GetMouseButtonUp(button))
            {
                canmove = false;
            }
            else if(canmove&&Input.GetMouseButton(button))
            {
                // 按下按钮且没有撞击到UI 时需要处理的事项写在里面。
                // The things that need to be handled when the button is pressed and the UI is not hit are written in it.
            }
        }
    }
}
#endregion

public static class InputExtension
{
    /// <summary>
    /// 判断Touch 按下时是否打到了 UI 组件。filter必须设定非空默认值，因为任意字符串StartWith（“”）永远返回 true
    /// <br>Determine whether the UI component is hit when touch begin. the filter must have a nonempty value as string.StartWith（“”）always reture true</br>
    /// </summary>
    public static bool IsRaycastUI(this Touch touch,string filter="#")=>Raycast(touch.position,filter);

    /// <summary>
    /// 判断指定按键按下时是否打到了 UI 组件。filter必须设定非空默认值，因为任意字符串StartWith（“”）永远返回 true
    /// <br>Determine whether the UI component is hit when the specified mouse button is pressed.the filter must have a nonempty value as string.StartWith（“”）always reture true</br>
    /// </summary>
    public static bool IsMouseRaycastUI(string filter="#")=>Raycast(Input.mousePosition,filter); 
    
    /// <summary>
    /// 执行射线检测确认是否打到了UI
    /// </summary>
    /// <param name="position">Touch 或者 光标所在的位置</param>
    /// <param name="filterPrefix">希望忽略的UI，有些情况下，从UI上开始拖拽也要旋转视野，如手游CF的狙击开镜+拖拽 ，注意：优先判断底层节点</param>
    /// <returns></returns>
    static bool Raycast(Vector2 position,string filterPrefix)
    {
        if (!EventSystem.current && !EventSystem.current.IsPointerOverGameObject()) return false;// 事件系统有效且射线有撞击到物体
        var data = new PointerEventData(EventSystem.current)
        {
            pressPosition = position,
            position = position
        };
        var list = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, list);
        return list.Count > 0 && list[0].module is GraphicRaycaster&&!list[0].gameObject.name.StartsWith(filterPrefix);
    }
}
