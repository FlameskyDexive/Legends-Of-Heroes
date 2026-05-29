using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ET.Client
{
    [EntitySystemOf(typeof(OperaComponent))]
    [FriendOf(typeof(OperaComponent))]
    public static partial class OperaComponentSystem
    {
        [EntitySystem]
        private static void Awake(this OperaComponent self)
        {
            self.mapMask = LayerMask.GetMask("Map");
        }

        [EntitySystem]
        private static void Update(this OperaComponent self)
        {
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                CodeLoader.Instance.Reload();
            }
        }

        // 每帧把当前帧收集的操作批量发送给服务端, 随后清空(摇杆移动/技能按键等)
        [EntitySystem]
        private static void LateUpdate(this OperaComponent self)
        {
            if (self.OperateInfos.Count == 0)
            {
                return;
            }

            self.OperateInfosTemp.Clear();
            self.OperateInfosTemp.AddRange(self.OperateInfos);

            C2M_Operation c2MOperation = C2M_Operation.Create();
            c2MOperation.OperateInfos = self.OperateInfosTemp;
            self.Root().GetComponent<ClientSenderComponent>().Send(c2MOperation);

            self.OperateInfos.Clear();
        }

        // 摇杆推动: 上报移动方向
        public static void OnMove(this OperaComponent self, Vector2 v2)
        {
            OperateInfo operateInfo = OperateInfo.Create();
            operateInfo.OperateType = (int)EOperateType.Move;
            operateInfo.InputType = (int)EInputType.KeyDown;
            operateInfo.Vec3 = new float3(v2.x, 0, v2.y);
            self.OperateInfos.Add(operateInfo);
        }

        // 摇杆松开: 上报停止移动
        public static void StopMove(this OperaComponent self)
        {
            OperateInfo operateInfo = OperateInfo.Create();
            operateInfo.OperateType = (int)EOperateType.Move;
            operateInfo.InputType = (int)EInputType.KeyUp;
            self.OperateInfos.Add(operateInfo);
        }

        public static void OnClickSkill1(this OperaComponent self)
        {
            OperateInfo operateInfo = OperateInfo.Create();
            operateInfo.OperateType = (int)EOperateType.Skill1;
            operateInfo.InputType = (int)EInputType.KeyDown;
            self.OperateInfos.Add(operateInfo);
        }

        public static void OnClickSkill2(this OperaComponent self)
        {
            OperateInfo operateInfo = OperateInfo.Create();
            operateInfo.OperateType = (int)EOperateType.Skill2;
            operateInfo.InputType = (int)EInputType.KeyDown;
            self.OperateInfos.Add(operateInfo);
        }

        /*
        private static async ETTask Test1(this OperaComponent self)
        {
            Log.Debug($"Croutine 1 start1 ");
            using (await self.Root().CoroutineLockComponent.Wait(1, 20000, 3000))
            {
                await self.Root().TimerComponent.WaitAsync(6000);
            }

            Log.Debug($"Croutine 1 end1");
        }
            
        private static async ETTask Test2(this OperaComponent self)
        {
            ETCancellationToken oldCancellationToken = await ETTask.GetContextAsync<ETCancellationToken>();
            Log.Debug($"Croutine 2 start2");
            using (await self.Root().CoroutineLockComponent.Wait(1, 20000, 3000))
            {
                await self.Root().TimerComponent.WaitAsync(1000);
            }
            Log.Debug($"Croutine 2 end2");
        }
        
        private static async ETTask TestCancelAfter(this OperaComponent self)
        {
            ETCancellationToken oldCancellationToken = await ETTask.GetContextAsync<ETCancellationToken>();
            
            Log.Debug($"TestCancelAfter start");
            ETCancellationToken newCancellationToken = new();
            await self.Root().TimerComponent.WaitAsync(3000).TimeoutAsync(newCancellationToken, 1000);
            if (newCancellationToken.IsCancel())
            {
                Log.Debug($"TestCancelAfter newCancellationToken is cancel!");
            }
            
            if (oldCancellationToken != null && !oldCancellationToken.IsCancel())
            {
                Log.Debug($"TestCancelAfter oldCancellationToken is not cancel!");
            }
            Log.Debug($"TestCancelAfter end");
        }
        */
    }
}