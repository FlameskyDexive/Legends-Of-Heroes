using System;
using System.Collections.Generic;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTreeLogNodeData : BTreeNodeData, IBTreeHandlerNodeData, IBTreeArgumentNodeData
    {
        public List<BTreeArgumentData> Arguments = new();

        public string NodeTypeId => BTreeBuiltinNodeTypes.Log;

        public string HandlerName => "Log";

        List<BTreeArgumentData> IBTreeArgumentNodeData.Arguments => this.Arguments;

        public BTreeLogNodeData()
        {
            this.NodeKind = BTreeNodeKind.Action;
        }

        public override BTreeNodeData Clone()
        {
            BTreeLogNodeData definition = this.CloneBaseTo(new BTreeLogNodeData());
            foreach (BTreeArgumentData argument in this.Arguments)
            {
                definition.Arguments.Add(argument?.Clone() ?? new BTreeArgumentData());
            }

            return definition;
        }
    }

    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTreeSetBlackboardNodeData : BTreeNodeData, IBTreeHandlerNodeData, IBTreeArgumentNodeData
    {
        public List<BTreeArgumentData> Arguments = new();

        public string NodeTypeId => BTreeBuiltinNodeTypes.SetBlackboard;

        public string HandlerName => "SetBlackboard";

        List<BTreeArgumentData> IBTreeArgumentNodeData.Arguments => this.Arguments;

        public BTreeSetBlackboardNodeData()
        {
            this.NodeKind = BTreeNodeKind.Action;
        }

        public override BTreeNodeData Clone()
        {
            BTreeSetBlackboardNodeData definition = this.CloneBaseTo(new BTreeSetBlackboardNodeData());
            foreach (BTreeArgumentData argument in this.Arguments)
            {
                definition.Arguments.Add(argument?.Clone() ?? new BTreeArgumentData());
            }

            return definition;
        }
    }

    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTreeSetBlackboardIfMissingData : BTreeNodeData, IBTreeHandlerNodeData, IBTreeArgumentNodeData
    {
        public List<BTreeArgumentData> Arguments = new();

        public string NodeTypeId => BTreeBuiltinNodeTypes.SetBlackboardIfMissing;

        public string HandlerName => "SetBlackboardIfMissing";

        List<BTreeArgumentData> IBTreeArgumentNodeData.Arguments => this.Arguments;

        public BTreeSetBlackboardIfMissingData()
        {
            this.NodeKind = BTreeNodeKind.Action;
        }

        public override BTreeNodeData Clone()
        {
            BTreeSetBlackboardIfMissingData definition = this.CloneBaseTo(new BTreeSetBlackboardIfMissingData());
            foreach (BTreeArgumentData argument in this.Arguments)
            {
                definition.Arguments.Add(argument?.Clone() ?? new BTreeArgumentData());
            }

            return definition;
        }
    }

    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTreeBlackboardExistsNodeData : BTreeNodeData, IBTreeHandlerNodeData, IBTreeArgumentNodeData
    {
        public List<BTreeArgumentData> Arguments = new();

        public string NodeTypeId => BTreeBuiltinNodeTypes.BlackboardExists;

        public string HandlerName => "BlackboardExists";

        List<BTreeArgumentData> IBTreeArgumentNodeData.Arguments => this.Arguments;

        public BTreeBlackboardExistsNodeData()
        {
            this.NodeKind = BTreeNodeKind.Condition;
        }

        public override BTreeNodeData Clone()
        {
            BTreeBlackboardExistsNodeData definition = this.CloneBaseTo(new BTreeBlackboardExistsNodeData());
            foreach (BTreeArgumentData argument in this.Arguments)
            {
                definition.Arguments.Add(argument?.Clone() ?? new BTreeArgumentData());
            }

            return definition;
        }
    }

    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTreeBlackboardCompareNodeData : BTreeNodeData, IBTreeHandlerNodeData, IBTreeArgumentNodeData
    {
        public List<BTreeArgumentData> Arguments = new();

        public string NodeTypeId => BTreeBuiltinNodeTypes.BlackboardCompare;

        public string HandlerName => "BlackboardCompare";

        List<BTreeArgumentData> IBTreeArgumentNodeData.Arguments => this.Arguments;

        public BTreeBlackboardCompareNodeData()
        {
            this.NodeKind = BTreeNodeKind.Condition;
        }

        public override BTreeNodeData Clone()
        {
            BTreeBlackboardCompareNodeData definition = this.CloneBaseTo(new BTreeBlackboardCompareNodeData());
            foreach (BTreeArgumentData argument in this.Arguments)
            {
                definition.Arguments.Add(argument?.Clone() ?? new BTreeArgumentData());
            }

            return definition;
        }
    }
}
