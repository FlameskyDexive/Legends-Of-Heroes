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

        // 当前帧收集的操作(普通结构, 非 MessageObject), LateUpdate 转成 proto 批量上报后清空
        public List<OperateInfoData> OperateInfos = new List<OperateInfoData>();
    }
}
