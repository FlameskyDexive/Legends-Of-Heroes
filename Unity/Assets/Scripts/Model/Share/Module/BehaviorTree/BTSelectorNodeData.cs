using System;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTSelectorNodeData : BTNodeData
    {
        public BTSelectorNodeData()
        {
            this.NodeKind = BehaviorTreeNodeKind.Selector;
        }

        public override BTNodeData Clone() => this.CloneBaseTo(new BTSelectorNodeData());
    }
}
