/// <summary>
/// 配表枚举类型定义专用
/// </summary>
using UnityEngine;
using System.Collections;


namespace ExcelParser
{

    public enum SkillMode
    {
        /// <summary>
        /// 客户端技能
        /// </summary>
        CLIENTSKILL,
        /// <summary>
        /// 脚本技能
        /// </summary>
        SCRIPTSKILL,
        /// <summary>
        /// 被动技能
        /// </summary>
        PASSIVESKILL,
        /// <summary>
        /// 普攻技能
        /// </summary>
        NORMALSKILL,

        UNUSED_SKILL,
    }
}