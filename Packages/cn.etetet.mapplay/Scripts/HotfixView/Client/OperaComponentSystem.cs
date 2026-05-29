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
        // Entity 里暂存的是普通结构 OperateInfoData, 这里才转成 proto 的 OperateInfo(池化)。
        [EntitySystem]
        private static void LateUpdate(this OperaComponent self)
        {
            if (self.OperateInfos.Count == 0)
            {
                return;
            }

            C2M_Operation c2MOperation = C2M_Operation.Create();
            foreach (OperateInfoData data in self.OperateInfos)
            {
                OperateInfo operateInfo = OperateInfo.Create();
                operateInfo.OperateType = data.OperateType;
                operateInfo.InputType = data.InputType;
                operateInfo.Vec3 = data.Vec3;
                c2MOperation.OperateInfos.Add(operateInfo);
            }
            self.Root().GetComponent<ClientSenderComponent>().Send(c2MOperation);

            self.OperateInfos.Clear();
        }

        // 摇杆推动: 上报移动方向
        public static void OnMove(this OperaComponent self, Vector2 v2)
        {
            self.OperateInfos.Add(new OperateInfoData
            {
                OperateType = (int)EOperateType.Move,
                InputType = (int)EInputType.KeyDown,
                Vec3 = new float3(v2.x, 0, v2.y),
            });
        }

        // 摇杆松开: 上报停止移动
        public static void StopMove(this OperaComponent self)
        {
            self.OperateInfos.Add(new OperateInfoData
            {
                OperateType = (int)EOperateType.Move,
                InputType = (int)EInputType.KeyUp,
            });
        }

        public static void OnClickSkill1(this OperaComponent self)
        {
            self.OperateInfos.Add(new OperateInfoData
            {
                OperateType = (int)EOperateType.Skill1,
                InputType = (int)EInputType.KeyDown,
            });
        }

        public static void OnClickSkill2(this OperaComponent self)
        {
            self.OperateInfos.Add(new OperateInfoData
            {
                OperateType = (int)EOperateType.Skill2,
                InputType = (int)EInputType.KeyDown,
            });
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