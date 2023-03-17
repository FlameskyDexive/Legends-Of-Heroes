using System.Collections;
using System.Collections.Generic;

namespace ET.Server
{
    [ComponentOf(typeof(Scene))]
    public class RoomManagerComponent : Entity, IAwake, IDestroy
    {
        public int RoomIdNum { get; set; }

        /// <summary>
        /// 后续改成双层字典，一个玩法一个字典存储房间，当前测试只先做一个玩法
        /// </summary>
        public Dictionary<long, Room> Rooms = new Dictionary<long, Room>();

    }
}