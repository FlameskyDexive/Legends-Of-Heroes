using System;
using System.Net;
using Unity.Mathematics;

namespace ET
{
    public static class Entry
    {
        public static void Start()
        {
            StartAsync().Coroutine();
        }
        
        private static async ETTask StartAsync()
        {
            WinPeriod.Init();

            // 注册Mongo type
            MongoRegister.Init();
            
            MemoryPackRegister.Init();
            
            // 注册Entity序列化器
            EntitySerializeRegister.Init();
            
            MongoRegister.RegisterStruct<float2>();
            MongoRegister.RegisterStruct<float3>();
            MongoRegister.RegisterStruct<float4>();
            MongoRegister.RegisterStruct<quaternion>();

            World.Instance.AddSingleton<SceneTypeSingleton, Type>(typeof(SceneType));
            World.Instance.AddSingleton<ObjectPool>();
            World.Instance.AddSingleton<IdGenerater>();
            World.Instance.AddSingleton<OpcodeType>();
            World.Instance.AddSingleton<NumericTypeEnum>();
            World.Instance.AddSingleton<MessageQueue>();
            
            LogMsg logMsg = World.Instance.AddSingleton<LogMsg>();
            logMsg.AddIgnore(typeof(C2G_Ping));
            logMsg.AddIgnore(typeof(G2C_Ping));
            logMsg.AddIgnore(typeof(MessageResponse));
            // 高频地图消息免逐条打印:编辑器 DEBUG 下 LogMsg 会对每条消息 Debug.Log(ToString=JSON序列化),
            // 球球大作战(大量单位移动/吃食物数值变化/食物创建移除)时每秒上百条 → Console 打印开销严重拖慢帧率。
            // 这些消息有专门 Handler,不需要逐条日志;关掉只影响调试打印,不影响逻辑。
            logMsg.AddIgnore(typeof(M2C_NumericChange));
            logMsg.AddIgnore(typeof(M2C_CreateUnits));
            logMsg.AddIgnore(typeof(M2C_RemoveUnits));
            logMsg.AddIgnore(typeof(M2C_PathfindingResult));
            logMsg.AddIgnore(typeof(M2C_Stop));
            
            
            // 创建需要reload的code singleton
            CodeTypes.Instance.CodeProcess();
            
            await World.Instance.AddSingleton<ConfigLoader>().LoadAsync();
            World.Instance.AddSingleton<NavmeshComponent>();

            int sceneType = SceneTypeSingleton.Instance.GetSceneType(Options.Instance.SceneName);
            await FiberManager.Instance.CreateMainFiber(sceneType, $"{Options.Instance.SceneName}@{Options.Instance.Process}@{Options.Instance.ReplicaIndex}");
        }
    }
}