using System;
using System.Collections.Generic;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTServiceNodeData : BTNodeData, IBTHandlerNodeData, IBTArgumentNodeData
    {
        public string TypeId = string.Empty;

        public string ServiceHandlerName = string.Empty;

        public int IntervalMilliseconds = 250;

        public List<BTArgumentData> Arguments = new();

        public string NodeTypeId => this.TypeId;

        public string HandlerName => this.ServiceHandlerName;

        List<BTArgumentData> IBTArgumentNodeData.Arguments => this.Arguments;

        public BTServiceNodeData()
        {
            this.NodeKind = BTNodeKind.Service;
        }

        public override BTNodeData Clone()
        {
            BTServiceNodeData definition = this.CloneBaseTo(new BTServiceNodeData
            {
                TypeId = this.TypeId,
                ServiceHandlerName = this.ServiceHandlerName,
                IntervalMilliseconds = this.IntervalMilliseconds,
            });

            foreach (BTArgumentData argument in this.Arguments)
            {
                definition.Arguments.Add(argument?.Clone() ?? new BTArgumentData());
            }

            return definition;
        }
    }
}
