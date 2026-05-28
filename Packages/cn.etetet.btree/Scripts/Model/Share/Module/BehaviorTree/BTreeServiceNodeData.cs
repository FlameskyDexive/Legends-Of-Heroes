using System;
using System.Collections.Generic;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTreeServiceNodeData : BTreeNodeData, IBTreeHandlerNodeData, IBTreeArgumentNodeData
    {
        public string TypeId = string.Empty;

        public string ServiceHandlerName = string.Empty;

        public int IntervalMilliseconds = 250;

        public List<BTreeArgumentData> Arguments = new();

        public string NodeTypeId => this.TypeId;

        public string HandlerName => this.ServiceHandlerName;

        List<BTreeArgumentData> IBTreeArgumentNodeData.Arguments => this.Arguments;

        public BTreeServiceNodeData()
        {
            this.NodeKind = BTreeNodeKind.Service;
        }

        public override BTreeNodeData Clone()
        {
            BTreeServiceNodeData definition = this.CloneBaseTo(new BTreeServiceNodeData
            {
                TypeId = this.TypeId,
                ServiceHandlerName = this.ServiceHandlerName,
                IntervalMilliseconds = this.IntervalMilliseconds,
            });

            foreach (BTreeArgumentData argument in this.Arguments)
            {
                definition.Arguments.Add(argument?.Clone() ?? new BTreeArgumentData());
            }

            return definition;
        }
    }
}
