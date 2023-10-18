using System;
using UnityEngine;

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
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000, self.mapMask))
                {
                    C2M_PathfindingResult c2MPathfindingResult = new C2M_PathfindingResult();
                    c2MPathfindingResult.Position = hit.point;
                    self.Root().GetComponent<ClientSenderCompnent>().Send(c2MPathfindingResult);
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                CodeLoader.Instance.Reload();
                return;
            }
        
            if (Input.GetKeyDown(KeyCode.T))
            {
                C2M_TransferMap c2MTransferMap = new();
                self.Root().GetComponent<ClientSenderCompnent>().Call(c2MTransferMap).Coroutine();
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
        
        public static void OnClickSkill1(this OperaComponent self)
        {
            //可在此处检测技能是否可释放（蓝量、CD、僵直等判定）
            Log.Info($"press skill1");
            // OperateInfo operateInfo = new OperateInfo() { OperateType = (int)EOperateType.Skill1, InputType = (int)EInputType.KeyDown };
            // self.OperateInfos.Add(operateInfo);
        }
        
        public static void OnClickSkill2(this OperaComponent self)
        {
            //可在此处检测技能是否可释放（蓝量、CD、僵直等判定）
            Log.Info($"press skill2");
            // OperateInfo operateInfo = new OperateInfo() { OperateType = (int)EOperateType.Skill2, InputType = (int)EInputType.KeyDown };
            // self.OperateInfos.Add(operateInfo);
        }
        
        public static void OnMove(this OperaComponent self, Vector2 v2)
        {
            Log.Info($"press joystick: {v2}");
            // C2M_JoystickMove c2mJoystickMove = new C2M_JoystickMove() { MoveForward = new float3(v2.x, 0, v2.y) };
            // self.ClientScene().GetComponent<PlayerSessionComponent>().Session.Send(c2mJoystickMove);
            // OperateInfo operateInfo = new OperateInfo(){OperateType = (int)EOperateType.Move, InputType = (int)EInputType.KeyDown, Vec3 = new float3(v2.x, 0, v2.y) };
            // self.OperateInfos.Add(operateInfo);
        }
    }
}