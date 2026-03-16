using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public abstract class BTNode
    {
        public int RuntimeNodeId;

        public string SourceNodeId = string.Empty;

        public string TreeId = string.Empty;

        public string TreeName = string.Empty;

        public BTNodeData Definition;

        public List<BTNode> Children = new();
    }

    [EnableClass]
    public sealed class BTRoot : BTNode
    {
    }

    [EnableClass]
    public abstract class BTComposite : BTNode
    {
    }

    [EnableClass]
    public abstract class BTDecorator : BTNode
    {
    }

    [EnableClass]
    public abstract class BTHandlerCallNode : BTNode
    {
        public string NodeTypeId = string.Empty;

        public string HandlerName = string.Empty;

        public List<BTArgumentData> Arguments = new();
    }

    [EnableClass]
    public sealed class BTActionCall : BTHandlerCallNode
    {
    }

    [EnableClass]
    public sealed class BTConditionCall : BTHandlerCallNode
    {
    }

    [EnableClass]
    public sealed class BTServiceCall : BTHandlerCallNode
    {
        public int IntervalMilliseconds = 250;
    }

    [EnableClass]
    public sealed class BTWait : BTNode
    {
        public int WaitMilliseconds = 1000;
    }

    [EnableClass]
    public sealed class BTBlackboardCondition : BTNode
    {
        public string BlackboardKey = string.Empty;

        public BTCompareOperator CompareOperator = BTCompareOperator.IsSet;

        public BTSerializedValue CompareValue = new();

        public BTAbortMode AbortMode = BTAbortMode.Self;
    }

    [EnableClass]
    public sealed class BTSubTreeCall : BTNode
    {
        public string SubTreeId = string.Empty;

        public string SubTreeName = string.Empty;

        public BTNode SubTreeRoot;
    }

    [EnableClass]
    public sealed class BTSequence : BTComposite
    {
    }

    [EnableClass]
    public sealed class BTSelector : BTComposite
    {
    }

    [EnableClass]
    public sealed class BTParallel : BTComposite
    {
        public BTParallelPolicy SuccessPolicy = BTParallelPolicy.RequireAll;

        public BTParallelPolicy FailurePolicy = BTParallelPolicy.RequireOne;
    }

    [EnableClass]
    public sealed class BTInverter : BTDecorator
    {
    }

    [EnableClass]
    public sealed class BTSucceeder : BTDecorator
    {
    }

    [EnableClass]
    public sealed class BTFailer : BTDecorator
    {
    }

    [EnableClass]
    public sealed class BTRepeater : BTDecorator
    {
        public int MaxLoopCount;
    }
}
