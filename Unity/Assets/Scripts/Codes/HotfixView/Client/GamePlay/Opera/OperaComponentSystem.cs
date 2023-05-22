using System;
using Unity.Mathematics;
using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof(OperaComponent))]
    public static class OperaComponentSystem
    {
        [ObjectSystem]
        public class OperaComponentAwakeSystem : AwakeSystem<OperaComponent>
        {
            protected override void Awake(OperaComponent self)
            {
                self.mapMask = LayerMask.GetMask("Map");
            }
        }

        [ObjectSystem]
        public class OperaComponentUpdateSystem: UpdateSystem<OperaComponent>
        {
            protected override void Update(OperaComponent self)
            {
                self.Update();

            }
        }

        [ObjectSystem]
        public class OperaComponentLateUpdateSystem : LateUpdateSystem<OperaComponent>
        {
            protected override void LateUpdate(OperaComponent self)
            {
                self.LateUpdate();
            }
        }

        private static void Update(this OperaComponent self)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000, self.mapMask))
                {
                    C2M_PathfindingResult c2MPathfindingResult = new C2M_PathfindingResult();
                    c2MPathfindingResult.Position = hit.point;
                    self.ClientScene().GetComponent<SessionComponent>().Session.Send(c2MPathfindingResult);
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                CodeLoader.Instance.LoadHotfix();
                EventSystem.Instance.Load();
                Log.Debug("hot reload success!");
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                C2M_TransferMap c2MTransferMap = new C2M_TransferMap();
                self.ClientScene().GetComponent<SessionComponent>().Session.Call(c2MTransferMap).Coroutine();
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                self.OnClickSkill1();
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                self.OnClickSkill2();   
            }
        }
        
        private static void LateUpdate(this OperaComponent self)
        {
            //每帧把当前帧收集的操作发送给服务端，随后清除
            if (self.OperateInfos.Count == 0)
                return;
            self.OperateInfosTemp.Clear();
            self.OperateInfosTemp.AddRange(self.OperateInfos);
            C2M_Operation c2MOperation = new C2M_Operation() { OperateInfos = self.OperateInfosTemp };
            self.ClientScene().GetComponent<SessionComponent>()?.Session?.Send(c2MOperation);
            self.OperateInfos.Clear();
        }

        public static void OnClickSkill1(this OperaComponent self)
        {
            //可在此处检测技能是否可释放（蓝量、CD、僵直等判定）
            Log.Info($"press skill1");
            OperateInfo operateInfo = new OperateInfo() { OperateType = (int)EOperateType.Skill1, InputType = (int)EInputType.KeyDown };
            self.OperateInfos.Add(operateInfo);
        }
        
        public static void OnClickSkill2(this OperaComponent self)
        {
            //可在此处检测技能是否可释放（蓝量、CD、僵直等判定）
            Log.Info($"press skill2");
            OperateInfo operateInfo = new OperateInfo() { OperateType = (int)EOperateType.Skill2, InputType = (int)EInputType.KeyDown };
            self.OperateInfos.Add(operateInfo);
        }
        
        public static void OnMove(this OperaComponent self, Vector2 v2)
        {
            Log.Info($"press joystick: {v2}");
            // C2M_JoystickMove c2mJoystickMove = new C2M_JoystickMove() { MoveForward = new float3(v2.x, 0, v2.y) };
            // self.ClientScene().GetComponent<PlayerSessionComponent>().Session.Send(c2mJoystickMove);
            OperateInfo operateInfo = new OperateInfo(){OperateType = (int)EOperateType.Move, InputType = (int)EInputType.KeyDown, Vec3 = new float3(v2.x, 0, v2.y) };
            self.OperateInfos.Add(operateInfo);
        }

    }
}