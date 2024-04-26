using System;
using System.Collections.Generic;

namespace ET.Server
{
    [FriendOf(typeof(RobotCaseComponent))]
    [EntitySystemOf(typeof(RobotCaseComponent))]
    public static partial class RobotCaseComponentSystem
    {
        [EntitySystem]
        public static void Awake(this RobotCaseComponent self)
        {
            Log.Console($"robot case awake");
        }
        [EntitySystem]
        public static void FixedUpdate(this RobotCaseComponent self)
        {
            //fixeupdate log test, current server logic frame:  DefineCore.FPS=20
            //fixedupdate log time interval is 50 ms.
            // 1711609211016
            // 1711609211066
            // 1711609211117
            // 1711609211167
            // 1711609211217
            // 1711609211268
            // 1711609211318
            // 1711609211368
            // 1711609211417
            // 1711609211467
            // 1711609211517
            // 1711609211567
            // 1711609211617
            // 1711609211666
            // 1711609211717
            // 1711609211767
            // 1711609211817
            // 1711609211867
            // 1711609211917
            // 1711609211966
            // 1711609212017
            // 1711609212067
            // 1711609212117
            // Log.Console($"{TimeInfo.Instance.ClientNow()}");
        }
        [EntitySystem]
        public static void Destroy(this RobotCaseComponent self)
        {
            Log.Console($"robot case destroy");
        }
        
        public static int GetN(this RobotCaseComponent self)
        {
            return ++self.N;
        }
        
        public static async ETTask<RobotCase> New(this RobotCaseComponent self)
        {
            await ETTask.CompletedTask;
            RobotCase robotCase = self.AddChild<RobotCase>();
            return robotCase;
        }
    }
}