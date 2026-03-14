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

        public List<BehaviorTreeArgumentDefinition> Arguments = new();

        public string NodeTypeId => this.TypeId;

        public string HandlerName => this.ServiceHandlerName;

        List<BehaviorTreeArgumentDefinition> IBTArgumentNodeData.Arguments => this.Arguments;

        public BTServiceNodeData()
        {
            this.NodeKind = BehaviorTreeNodeKind.Service;
        }

        public override BTNodeData Clone()
        {
            BTServiceNodeData definition = this.CloneBaseTo(new BTServiceNodeData
            {
                TypeId = this.TypeId,
                ServiceHandlerName = this.ServiceHandlerName,
                IntervalMilliseconds = this.IntervalMilliseconds,
            });

            foreach (BehaviorTreeArgumentDefinition argument in this.Arguments)
            {
                definition.Arguments.Add(argument?.Clone() ?? new BehaviorTreeArgumentDefinition());
            }

            return definition;
        }
    }
}
