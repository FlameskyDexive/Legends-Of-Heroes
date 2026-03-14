using System;
using System.Collections.Generic;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTActionNodeData : BTNodeData, IBTHandlerNodeData, IBTArgumentNodeData
    {
        public string TypeId = string.Empty;

        public string ActionHandlerName = string.Empty;

        public List<BTArgumentData> Arguments = new();

        public string NodeTypeId => this.TypeId;

        public string HandlerName => this.ActionHandlerName;

        List<BTArgumentData> IBTArgumentNodeData.Arguments => this.Arguments;

        public BTActionNodeData()
        {
            this.NodeKind = BTNodeKind.Action;
        }

        public override BTNodeData Clone()
        {
            BTActionNodeData definition = this.CloneBaseTo(new BTActionNodeData
            {
                TypeId = this.TypeId,
                ActionHandlerName = this.ActionHandlerName,
            });

            foreach (BTArgumentData argument in this.Arguments)
            {
                definition.Arguments.Add(argument?.Clone() ?? new BTArgumentData());
            }

            return definition;
        }
    }
}
