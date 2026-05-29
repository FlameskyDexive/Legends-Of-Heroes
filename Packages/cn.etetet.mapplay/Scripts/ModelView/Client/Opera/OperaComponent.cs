using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET.Client
{
	[ComponentOf(typeof(Scene))]
	public class OperaComponent: Entity, IAwake, IUpdate, ILateUpdate
    {
        public Vector3 ClickPoint;

	    public int mapMask;

        // 当前帧收集的操作(摇杆移动、技能按键等), LateUpdate 批量上报后清空
        public List<OperateInfo> OperateInfos = new List<OperateInfo>();
        public List<OperateInfo> OperateInfosTemp = new List<OperateInfo>();
    }
}
