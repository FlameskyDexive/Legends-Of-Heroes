using System;
using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public sealed class BTEnv
    {
        public EntityRef<Entity> Owner;

        public long RuntimeId;

        public BTBlackboard Blackboard;

        public BTExecutionSession Session;

        public BTDefinition CurrentTree;

        public string TreeId = string.Empty;

        public string TreeName = string.Empty;

        public readonly Stack<int> CallPath = new();

        public readonly Dictionary<int, BTNodeRuntimeState> States = new();

        public readonly Dictionary<string, object> SharedData = new(StringComparer.OrdinalIgnoreCase);

        public readonly BTExecutionContext ExecutionContext = new();

        public BTExecutionContext BindContext(BTNode node)
        {
            return this.ExecutionContext.Configure(this.RuntimeId, node?.TreeId ?? this.TreeId, node?.TreeName ?? this.TreeName, this.Owner, this.Blackboard);
        }
    }
}
