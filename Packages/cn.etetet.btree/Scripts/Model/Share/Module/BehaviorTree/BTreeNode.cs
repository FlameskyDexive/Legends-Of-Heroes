using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public abstract class BTreeNode
    {
        public int RuntimeNodeId;

        public string SourceNodeId = string.Empty;

        public string TreeId = string.Empty;

        public string TreeName = string.Empty;

        public BTreeNodeData Definition;

        public List<BTreeNode> Children = new();
    }

    [EnableClass]
    public sealed class BTreeRoot : BTreeNode
    {
    }

    [EnableClass]
    public abstract class BTreeComposite : BTreeNode
    {
    }

    [EnableClass]
    public abstract class BTreeDecorator : BTreeNode
    {
    }

    [EnableClass]
    public abstract class BTreeAction : BTreeNode
    {
    }

    [EnableClass]
    public abstract class BTreeCondition : BTreeNode
    {
    }

    [EnableClass]
    public abstract class BTreeService : BTreeNode
    {
    }

    [EnableClass]
    public abstract class BTreeHandlerCallNode : BTreeNode
    {
    }

    [EnableClass]
    public sealed class BTreeActionCall : BTreeAction
    {
    }

    [EnableClass]
    public sealed class BTreeConditionCall : BTreeCondition
    {
    }

    [EnableClass]
    public sealed class BTreeServiceCall : BTreeService
    {
    }

    [EnableClass]
    public sealed class BTreeLog : BTreeAction
    {
    }

    [EnableClass]
    public sealed class BTreeSetBlackboard : BTreeAction
    {
    }

    [EnableClass]
    public sealed class BTreeSetBlackboardIfMissing : BTreeAction
    {
    }

    [EnableClass]
    public sealed class BTreeBlackboardExists : BTreeCondition
    {
    }

    [EnableClass]
    public sealed class BTreeBlackboardCompare : BTreeCondition
    {
    }

    [EnableClass]
    public sealed class BTreePatrol : BTreeAction
    {
    }

    [EnableClass]
    public sealed class BTreeHasPatrolPath : BTreeCondition
    {
    }

    [EnableClass]
    public sealed class BTreeWait : BTreeNode
    {
    }

    [EnableClass]
    public sealed class BTreeBlackboardCondition : BTreeNode
    {
    }

    [EnableClass]
    public sealed class BTreeSubTreeCall : BTreeNode
    {
        public BTreeNode SubTreeRoot;
    }

    [EnableClass]
    public sealed class BTreeSequence : BTreeComposite
    {
    }

    [EnableClass]
    public sealed class BTreeSelector : BTreeComposite
    {
    }

    [EnableClass]
    public sealed class BTreeParallel : BTreeComposite
    {
    }

    [EnableClass]
    public sealed class BTreeInverter : BTreeDecorator
    {
    }

    [EnableClass]
    public sealed class BTreeSucceeder : BTreeDecorator
    {
    }

    [EnableClass]
    public sealed class BTreeFailer : BTreeDecorator
    {
    }

    [EnableClass]
    public sealed class BTreeRepeater : BTreeDecorator
    {
    }
}
