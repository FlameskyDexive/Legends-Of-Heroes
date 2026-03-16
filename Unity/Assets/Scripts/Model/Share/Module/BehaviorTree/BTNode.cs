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
    public abstract class BTAction : BTNode
    {
    }

    [EnableClass]
    public abstract class BTCondition : BTNode
    {
    }

    [EnableClass]
    public abstract class BTService : BTNode
    {
    }

    [EnableClass]
    public abstract class BTHandlerCallNode : BTNode
    {
    }

    [EnableClass]
    public sealed class BTActionCall : BTAction
    {
    }

    [EnableClass]
    public sealed class BTConditionCall : BTCondition
    {
    }

    [EnableClass]
    public sealed class BTServiceCall : BTService
    {
    }

    [EnableClass]
    public sealed class BTLog : BTAction
    {
    }

    [EnableClass]
    public sealed class BTSetBlackboard : BTAction
    {
    }

    [EnableClass]
    public sealed class BTSetBlackboardIfMissing : BTAction
    {
    }

    [EnableClass]
    public sealed class BTBlackboardExists : BTCondition
    {
    }

    [EnableClass]
    public sealed class BTBlackboardCompare : BTCondition
    {
    }

    [EnableClass]
    public sealed class BTPatrol : BTAction
    {
    }

    [EnableClass]
    public sealed class BTHasPatrolPath : BTCondition
    {
    }

    [EnableClass]
    public sealed class BTWait : BTNode
    {
    }

    [EnableClass]
    public sealed class BTBlackboardCondition : BTNode
    {
    }

    [EnableClass]
    public sealed class BTSubTreeCall : BTNode
    {
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
    }
}
