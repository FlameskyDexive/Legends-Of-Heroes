using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class NonBreakingSpaceTextComponent : MonoBehaviour
{
    public static readonly string no_breaking_space = "\u00A0";

    protected TextMeshProUGUI text;
    // Use this for initialization
    void Awake()
    {
        text = this.GetComponent<TextMeshProUGUI>();
        text.RegisterDirtyVerticesCallback(OnTextChange);
    }

    public void OnTextChange()
    {
        if (text.text.Contains(" "))
        {
            text.text = text.text.Replace(" ", no_breaking_space);
        }
    }

}