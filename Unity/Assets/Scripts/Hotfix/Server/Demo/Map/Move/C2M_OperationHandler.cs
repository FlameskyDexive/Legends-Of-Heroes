using System.Collections.Generic;
using Unity.Mathematics;

namespace ET.Server
{
    [MessageLocationHandler(SceneType.Map)]
    public class C2M_OperationHandler : MessageLocationHandler<Unit, C2M_Operation>
    {
        protected override async ETTask Run(Unit unit, C2M_Operation message)
        {
            if (message.OperateInfos == null || message.OperateInfos.Count == 0)
            {
                Log.Error($"reveice null operate info");
                return;
            }

            M2C_Operation m2COperation = M2C_Operation.Create();
            m2COperation.OperateInfos = new List<OperateReplyInfo>();
            
            foreach (OperateInfo operateInfo in message.OperateInfos)
            {
                EOperateType operateType = (EOperateType)operateInfo.OperateType;
                switch (operateType)
                {
                    case EOperateType.Move:
                    {
                        //收到移动消息，往前移动，如果有地形，需要判定前方位置是否可以移动。
                        float speed = unit.GetComponent<NumericComponent>().GetAsFloat(NumericType.Speed);
                        speed = speed == 0 ? 3 : speed;
                        float3 v3 = unit.Position + operateInfo.Vec3 * speed / 30f;
                        unit.Position = v3;
                        unit.Forward = operateInfo.Vec3;
                        // unit.Forward = new float3(0, message.MoveForward.y, message.MoveForward.x);
                        M2C_JoystickMove m2CJoystickMove = M2C_JoystickMove.Create();

                        m2CJoystickMove.Position = unit.Position;
                        m2CJoystickMove.MoveForward = unit.Forward; 
                        m2CJoystickMove.Id = unit.Id;

                        MapMessageHelper.Broadcast(unit, m2CJoystickMove);
                        break;
                    }
                    case EOperateType.Attack:
                    {
                        
                        break;
                    }
                    case EOperateType.Skill1:
                    {
                        //主动技能1
                        if (unit?.GetComponent<SkillComponent>()?.SpellSkill(ESkillAbstractType.ActiveSkill) == true)
                        {
                            OperateReplyInfo info = OperateReplyInfo.Create();
                            info.OperateType = (int)operateType;
                            info.Status = 0;
                            m2COperation.OperateInfos.Add(info);
                        }
                        break;
                    }
                    case EOperateType.Skill2:
                    {
                        //主动技能2
                        if (unit?.GetComponent<SkillComponent>()?.SpellSkill(ESkillAbstractType.ActiveSkill, 1) == true)
                        {
                            OperateReplyInfo info = OperateReplyInfo.Create();
                            info.OperateType = (int)operateType; info.Status = 0;
                            m2COperation.OperateInfos.Add(info);
                        }
                        break;
                    }
                }
            }
            if(m2COperation.OperateInfos?.Count > 0)
                MapMessageHelper.SendToClient(unit, m2COperation);

            await ETTask.CompletedTask;
        }
    }
    
}