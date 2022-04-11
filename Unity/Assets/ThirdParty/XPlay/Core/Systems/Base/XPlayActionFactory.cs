/****************************************************
    Author:		Flamesky
    Mail:		flamesky@live.com
    Date:		2022/3/15 16:44:48
    Function:	Nothing
*****************************************************/

using System;
using System.Collections.Generic;

namespace XPlay.Runtime
{
    public class XPlayActionFactory
    {
        /// <summary>
        /// ÊÂ¼þÆ¬×Öµä
        /// </summary>
        public static Dictionary<EActionType, Type> XPlayActionTypes = new Dictionary<EActionType, Type>()
        {
            {EActionType.Log, typeof(XPlayLogAction)},
            {EActionType.Event, typeof(XPlayEvent)},
            {EActionType.AttackEvent, typeof(XPlayAttackEvent)},
            {EActionType.PlayAnimation, typeof(XPlayPlayAnimation)},
        };

        public static XPlayIDirectable CreateActionType(EActionType actionType)
        {
            if (!XPlayActionTypes.ContainsKey(actionType))
            {
                return null;
            }
            return Activator.CreateInstance(XPlayActionTypes[actionType]) as XPlayIDirectable;
        }
    }
}

