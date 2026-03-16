using System;
using System.Collections.Generic;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTLogNodeData : BTNodeData, IBTHandlerNodeData, IBTArgumentNodeData
    {
        public List<BTArgumentData> Arguments = new();

        public string NodeTypeId => BTBuiltinNodeTypes.Log;

        public string HandlerName => "Log";

        List<BTArgumentData> IBTArgumentNodeData.Arguments => this.Arguments;

        public BTLogNodeData()
        {
            this.NodeKind = BTNodeKind.Action;
        }

        public override BTNodeData Clone()
        {
            BTLogNodeData definition = this.CloneBaseTo(new BTLogNodeData());
            foreach (BTArgumentData argument in this.Arguments)
            {
                definition.Arguments.Add(argument?.Clone() ?? new BTArgumentData());
            }

            return definition;
        }
    }

    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTSetBlackboardNodeData : BTNodeData, IBTHandlerNodeData, IBTArgumentNodeData
    {
        public List<BTArgumentData> Arguments = new();

        public string NodeTypeId => BTBuiltinNodeTypes.SetBlackboard;

        public string HandlerName => "SetBlackboard";

        List<BTArgumentData> IBTArgumentNodeData.Arguments => this.Arguments;

        public BTSetBlackboardNodeData()
        {
            this.NodeKind = BTNodeKind.Action;
        }

        public override BTNodeData Clone()
        {
            BTSetBlackboardNodeData definition = this.CloneBaseTo(new BTSetBlackboardNodeData());
            foreach (BTArgumentData argument in this.Arguments)
            {
                definition.Arguments.Add(argument?.Clone() ?? new BTArgumentData());
            }

            return definition;
        }
    }

    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTSetBlackboardIfMissingData : BTNodeData, IBTHandlerNodeData, IBTArgumentNodeData
    {
        public List<BTArgumentData> Arguments = new();

        public string NodeTypeId => BTBuiltinNodeTypes.SetBlackboardIfMissing;

        public string HandlerName => "SetBlackboardIfMissing";

        List<BTArgumentData> IBTArgumentNodeData.Arguments => this.Arguments;

        public BTSetBlackboardIfMissingData()
        {
            this.NodeKind = BTNodeKind.Action;
        }

        public override BTNodeData Clone()
        {
            BTSetBlackboardIfMissingData definition = this.CloneBaseTo(new BTSetBlackboardIfMissingData());
            foreach (BTArgumentData argument in this.Arguments)
            {
                definition.Arguments.Add(argument?.Clone() ?? new BTArgumentData());
            }

            return definition;
        }
    }

    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTBlackboardExistsNodeData : BTNodeData, IBTHandlerNodeData, IBTArgumentNodeData
    {
        public List<BTArgumentData> Arguments = new();

        public string NodeTypeId => BTBuiltinNodeTypes.BlackboardExists;

        public string HandlerName => "BlackboardExists";

        List<BTArgumentData> IBTArgumentNodeData.Arguments => this.Arguments;

        public BTBlackboardExistsNodeData()
        {
            this.NodeKind = BTNodeKind.Condition;
        }

        public override BTNodeData Clone()
        {
            BTBlackboardExistsNodeData definition = this.CloneBaseTo(new BTBlackboardExistsNodeData());
            foreach (BTArgumentData argument in this.Arguments)
            {
                definition.Arguments.Add(argument?.Clone() ?? new BTArgumentData());
            }

            return definition;
        }
    }

    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTBlackboardCompareNodeData : BTNodeData, IBTHandlerNodeData, IBTArgumentNodeData
    {
        public List<BTArgumentData> Arguments = new();

        public string NodeTypeId => BTBuiltinNodeTypes.BlackboardCompare;

        public string HandlerName => "BlackboardCompare";

        List<BTArgumentData> IBTArgumentNodeData.Arguments => this.Arguments;

        public BTBlackboardCompareNodeData()
        {
            this.NodeKind = BTNodeKind.Condition;
        }

        public override BTNodeData Clone()
        {
            BTBlackboardCompareNodeData definition = this.CloneBaseTo(new BTBlackboardCompareNodeData());
            foreach (BTArgumentData argument in this.Arguments)
            {
                definition.Arguments.Add(argument?.Clone() ?? new BTArgumentData());
            }

            return definition;
        }
    }
}
