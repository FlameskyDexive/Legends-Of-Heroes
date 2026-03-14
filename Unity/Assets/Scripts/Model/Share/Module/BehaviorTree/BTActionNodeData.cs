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

        public List<BehaviorTreeArgumentDefinition> Arguments = new();

        public string NodeTypeId => this.TypeId;

        public string HandlerName => this.ActionHandlerName;

        List<BehaviorTreeArgumentDefinition> IBTArgumentNodeData.Arguments => this.Arguments;

        public BTActionNodeData()
        {
            this.NodeKind = BehaviorTreeNodeKind.Action;
        }

        public override BTNodeData Clone()
        {
            BTActionNodeData definition = this.CloneBaseTo(new BTActionNodeData
            {
                TypeId = this.TypeId,
                ActionHandlerName = this.ActionHandlerName,
            });

            foreach (BehaviorTreeArgumentDefinition argument in this.Arguments)
            {
                definition.Arguments.Add(argument?.Clone() ?? new BehaviorTreeArgumentDefinition());
            }

            return definition;
        }
    }
}
