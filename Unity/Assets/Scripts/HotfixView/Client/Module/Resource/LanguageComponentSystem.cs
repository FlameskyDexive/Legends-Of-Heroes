using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    
    [FriendOf(typeof(LanguageComponent))]
    [EntitySystemOf(typeof(LanguageComponent))]
    public static partial class LanguageComponentSystem
    {
        [EntitySystem]
        public static void Awake(this LanguageComponent self)
        {
            LanguageComponent.Instance = self;
            self.curLangType = (ELanguageType)PlayerPrefs.GetInt(CacheKeys.CurLangType, 0);

            var res = LanguageConfigCategory.Instance.GetAll();
            self.langTextKeyDic = new Dictionary<string, string>();
            foreach (var item in res)
            {
                string value = item.Value.Chinese;
                if (self.curLangType == ELanguageType.English)
                {
                    value = item.Value.English;
                }
                self.langTextKeyDic.Add(item.Value.Key, value);
            }

            LanguageBridge.Instance.langTextKeyDic = self.langTextKeyDic;
            self.AddSystemFonts();
        }

        [EntitySystem]
        public static void Destroy(this LanguageComponent self)
        {
            LanguageComponent.Instance = null;
            self.langTextKeyDic.Clear();
            self.langTextKeyDic = null;
            LanguageComponent.Instance.langTextKeyDic = null;
        }

        public static string GetText(this LanguageComponent self, string key)
        {
            if (!self.langTextKeyDic.TryGetValue(key, out var value))
            {
                return key;
            }
            return value;
        }

        /// <summary>
        /// 根据key取多语言取不到返回key
        /// </summary>
        /// <param name="self"></param>
        /// <param name="key"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public static string GetParamText(this LanguageComponent self, string key, params object[] paras)
        {
            if (!self.langTextKeyDic.TryGetValue(key, out var value))
            {
                return key;
            }
            if (paras != null)
                return string.Format(value, paras);
            else
                return value;
        }
        /// <summary>
        /// 取不到返回key
        /// </summary>
        /// <param name="self"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool TryGetText(this LanguageComponent self, string key, out string result)
        {
            if (!self.langTextKeyDic.TryGetValue(key, out result))
            {
                result = key;
                return false;
            }
            return true;
        }
        /// <summary>
        /// 切换语言,外部接口
        /// </summary>
        /// <param name="langType"></param>
        public static void SwitchLanguage(this LanguageComponent self, ELanguageType langType)
        {
            if (self.curLangType == langType) 
                return;
            //修改当前语言
            PlayerPrefs.SetInt(CacheKeys.CurLangType, (int)langType);
            self.curLangType = langType;
            self.langTextKeyDic.Clear();
            var res = LanguageConfigCategory.Instance.GetAll();
            foreach (var item in res)
            {
                string value = item.Value.Chinese;
                if (self.curLangType == ELanguageType.English)
                {
                    value = item.Value.English;
                }
                self.langTextKeyDic.Add(item.Value.Key, value);
            }

            LanguageBridge.Instance.langTextKeyDic = self.langTextKeyDic;

            LanguageBridge.Instance.OnLanguageChange();
        }


        public static ELanguageType GetCurLanguage(this LanguageComponent self)
        {
            return self.curLangType;
        }

        #region 添加系统字体

        /// <summary>
        /// 需要就添加
        /// </summary>
        public static void AddSystemFonts(this LanguageComponent self)
        {
#if UNITY_EDITOR||UNITY_STANDALONE_WIN
            string[] fonts = new[] { "msyhl" };//微软雅黑细体
#elif UNITY_ANDROID
            string[] fonts = new[] {
                "NotoSansDevanagari-Regular",//天城体梵文
                "NotoSansThai-Regular",        //泰文
                "NotoSerifHebrew-Regular",     //希伯来文
                "NotoSansSymbols-Regular-Subsetted",  //符号
                "NotoSansCJK-Regular"          //中日韩
            };
#elif UNITY_IOS
            string[] fonts = new[] {
                "DevanagariSangamMN",  //天城体梵文
                "AppleSDGothicNeo",    //韩文，包含日文，部分中文
                "Thonburi",            //泰文
                "ArialHB"              //希伯来文
            };
#else
            string[] fonts = new string[0];
#endif
            TextMeshFontAssetManager.Instance.AddWithOSFont(fonts);
        }

        #endregion
    }
}
