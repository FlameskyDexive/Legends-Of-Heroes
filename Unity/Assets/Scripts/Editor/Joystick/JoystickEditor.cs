using UnityEditor;
using UnityEngine;
using System.Reflection;
using System;
using System.Linq;
using UnityEngine.UI;
using System.IO;

public class JoystickEditor 
{
    /// <summary>
    /// 不要使用脚本动态调用此接口
    /// </summary>
    /// <param name="cmd"></param>
    [MenuItem("GameObject/UI/Joystick", false, priority = 2088)]
    public static void AddJoystick(MenuCommand cmd)
    {
        // 搭建 Joystick 组件
        GameObject go = new GameObject("Joystick", typeof(Image));
        GameObject bg = new GameObject("BackGround", typeof(Image));
        var knob = new GameObject("Knob", typeof(Image));
        var arrow = new GameObject("Arrow", typeof(Image));

        bg.transform.SetParent(go.transform, false);
        knob.transform.SetParent(bg.transform, false);
        arrow.transform.SetParent(bg.transform, false);
        arrow.SetActive(false);

        // 挂载 Joystick 组件并初始化引用
        var joystick = go.AddComponent<Joystick>();
        joystick.backGround = bg.transform;
        joystick.knob = knob.transform;
        joystick.arrow = arrow.transform;

        //挂载贴图
        MonoScript mono = MonoScript.FromMonoBehaviour(joystick);
        var path = AssetDatabase.GetAssetPath(mono);
        path = path.Substring(0,path.IndexOf("Runtime"));
        path = $"{path}Texture/JoyStick.png";
        var texture = AssetDatabase.LoadAllAssetsAtPath(path)
                                   .OfType<Sprite>()
                                   .OrderBy(v=>v.name)
                                   .ToArray();
        var image = bg.GetComponent<Image>();
        image.sprite = texture[0];
        image = knob.GetComponent<Image>();
        image.sprite = texture[2];
        image = arrow.GetComponent<Image>();
        image.sprite = texture[1];

        //调整大小
        var rectt = go.GetComponent<RectTransform>();
        rectt.sizeDelta = new Vector2(200, 240);
        image = go.GetComponent<Image>();
        image.color = new Color32(255, 255, 255, 25);
        rectt = bg.GetComponent<RectTransform>();
        rectt.sizeDelta = Vector2.one * 126;
        rectt = knob.GetComponent<RectTransform>();
        rectt.sizeDelta = Vector2.one * 48;
        rectt = arrow.GetComponent<RectTransform>();
        rectt.sizeDelta = new Vector2(45, 100);
        rectt.pivot = new Vector2(-1, 0.5f);

        Undo.RegisterCreatedObjectUndo(go, $"CreatJoystick{Time.frameCount}");
        // 以下代码通过反射获取 UGUI 中新增 UI 组件的体验：会自动构建 UI 运行环境
        try
        {
            Type type = Type.GetType("UnityEditor.UI.MenuOptions,UnityEditor.UI.dll", true);
            var method = type.GetMethod("PlaceUIElementRoot", BindingFlags.Static | BindingFlags.NonPublic);
            method.Invoke(null, new object[] { go, cmd });
        }
        catch (Exception e)
        {
            Debug.Log($"{nameof(JoystickEditor)}: 挂载组件失败，绝逼是 API 变更!");
            throw e;
        }
    }
}
