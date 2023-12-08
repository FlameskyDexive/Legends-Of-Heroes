using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ET
{
    public class LanguageText : MonoBehaviour
    {
        [Tooltip("多语言Text组件")]
        public string key;
        private Text m_Text;
        private TMP_Text m_MeshText;
        void Awake()
        {
            m_Text = GetComponent<Text>();
            m_MeshText = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            OnSwitchLanguage();
            LanguageBridge.Instance.OnLanguageChangeEvt += OnSwitchLanguage;
        }

        private void OnDisable()
        {
            LanguageBridge.Instance.OnLanguageChangeEvt -= OnSwitchLanguage;
        }

        private void OnSwitchLanguage()
        {
            if (m_Text != null)
                m_Text.text = LanguageBridge.Instance.GetText(key);
            if (m_MeshText != null)
                m_MeshText.text = LanguageBridge.Instance.GetText(key);
        }
    }
}


