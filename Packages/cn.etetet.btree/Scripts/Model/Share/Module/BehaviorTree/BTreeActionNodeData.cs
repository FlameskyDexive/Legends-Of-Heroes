using System;
using System.Collections.Generic;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTreeActionNodeData : BTreeNodeData, IBTreeHandlerNodeData, IBTreeArgumentNodeData
    {
        public string TypeId = string.Empty;

        public string ActionHandlerName = string.Empty;

        public List<BTreeArgumentData> Arguments = new();

        public string NodeTypeId => this.TypeId;

        public string HandlerName => this.ActionHandlerName;

        List<BTreeArgumentData> IBTreeArgumentNodeData.Arguments => this.Arguments;

        public BTreeActionNodeData()
        {
            this.NodeKind = BTreeNodeKind.Action;
        }

        public override BTreeNodeData Clone()
        {
            BTreeActionNodeData definition = this.CloneBaseTo(new BTreeActionNodeData
            {
                TypeId = this.TypeId,
                ActionHandlerName = this.ActionHandlerName,
            });

            foreach (BTreeArgumentData argument in this.Arguments)
            {
                definition.Arguments.Add(argument?.Clone() ?? new BTreeArgumentData());
            }

            return definition;
        }
    }
}
