using System.Collections;
using System.Collections.Generic;

namespace ET
{
    [ComponentOf(typeof(Scene))]
    public class LanguageComponent : Entity,IAwake,IDestroy
    {
        [StaticField]
        public static LanguageComponent Instance;
        //语言类型枚举
       
        public ELanguageType curLangType;
        public Dictionary<string, string> langTextKeyDic;
        // public Dictionary<long, Entity> I18NEntity;
    }

}