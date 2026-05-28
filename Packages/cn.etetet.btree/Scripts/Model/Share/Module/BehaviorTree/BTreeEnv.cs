using System;
using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public sealed class BTreeEnv
    {
        public EntityRef<Entity> Owner;

        public long RuntimeId;

        public BTreeBlackboard Blackboard;

        public BTreeExecutionSession Session;

        public BTreeDefinition CurrentTree;

        public string TreeId = string.Empty;

        public string TreeName = string.Empty;

        public readonly Stack<int> CallPath = new();

        public readonly Dictionary<int, BTreeNodeRuntimeState> States = new();

        public readonly Dictionary<string, object> SharedData = new(StringComparer.OrdinalIgnoreCase);

        public readonly BTreeExecutionContext ExecutionContext = new();

        public BTreeExecutionContext BindContext(BTreeNode node)
        {
            return this.ExecutionContext.Configure(this.RuntimeId, node?.TreeId ?? this.TreeId, node?.TreeName ?? this.TreeName, this.Owner, this.Blackboard);
        }
    }
}
