using System;
using System.Collections.Generic;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTreeConditionNodeData : BTreeNodeData, IBTreeHandlerNodeData, IBTreeArgumentNodeData
    {
        public string TypeId = string.Empty;

        public string ConditionHandlerName = string.Empty;

        public List<BTreeArgumentData> Arguments = new();

        public string NodeTypeId => this.TypeId;

        public string HandlerName => this.ConditionHandlerName;

        List<BTreeArgumentData> IBTreeArgumentNodeData.Arguments => this.Arguments;

        public BTreeConditionNodeData()
        {
            this.NodeKind = BTreeNodeKind.Condition;
        }

        public override BTreeNodeData Clone()
        {
            BTreeConditionNodeData definition = this.CloneBaseTo(new BTreeConditionNodeData
            {
                TypeId = this.TypeId,
                ConditionHandlerName = this.ConditionHandlerName,
            });

            foreach (BTreeArgumentData argument in this.Arguments)
            {
                definition.Arguments.Add(argument?.Clone() ?? new BTreeArgumentData());
            }

            return definition;
        }
    }
}
