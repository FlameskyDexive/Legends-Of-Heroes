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

        // 摇杆移动上报节流:上次上报 Move 的客户端时间(ms)。摇杆按住时 OnValueChanged 每帧触发,
        // 若每帧都发 C2M_Operation 会超过服务端 MessageStatisticsComponent 的每秒上限(网关 >上限即断连),
        // 故 OnMove 按 MoveSendIntervalMs 节流(取最新方向)。
        public long LastMoveSendTime;
    }
}
