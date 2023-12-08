using System;
using System.Collections.Generic;

namespace ET
{
    public class LanguageBridge
    {
        public static LanguageBridge Instance { get; private set; } = new LanguageBridge();

        public Dictionary<string, string> langTextKeyDic;
        public event Action OnLanguageChangeEvt;
        public void OnLanguageChange()
        {
            OnLanguageChangeEvt?.Invoke();
        }
        /// <summary>
        /// 通过key获取多语言文本
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public string GetText(string key)
        {
            if (this.langTextKeyDic.ContainsKey(key))
            {
                return this.langTextKeyDic[key];
            }
            Log.Error("多语言未配置：" + key);
            return key;
        }

    }

}

