using System;
using System.Collections.Generic;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTConditionNodeData : BTNodeData, IBTHandlerNodeData, IBTArgumentNodeData
    {
        public string TypeId = string.Empty;

        public string ConditionHandlerName = string.Empty;

        public List<BTArgumentData> Arguments = new();

        public string NodeTypeId => this.TypeId;

        public string HandlerName => this.ConditionHandlerName;

        List<BTArgumentData> IBTArgumentNodeData.Arguments => this.Arguments;

        public BTConditionNodeData()
        {
            this.NodeKind = BTNodeKind.Condition;
        }

        public override BTNodeData Clone()
        {
            BTConditionNodeData definition = this.CloneBaseTo(new BTConditionNodeData
            {
                TypeId = this.TypeId,
                ConditionHandlerName = this.ConditionHandlerName,
            });

            foreach (BTArgumentData argument in this.Arguments)
            {
                definition.Arguments.Add(argument?.Clone() ?? new BTArgumentData());
            }

            return definition;
        }
    }
}
