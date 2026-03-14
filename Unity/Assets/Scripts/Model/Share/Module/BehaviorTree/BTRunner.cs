using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public sealed class BTRunner
    {
        public EntityRef<Entity> Owner;

        public Dictionary<string, BTDefinition> TreeIdMap = new();

        public Dictionary<string, BTDefinition> TreeNameMap = new(System.StringComparer.OrdinalIgnoreCase);

        public Dictionary<string, BTNodeState> NodeStates = new();

        public BTRuntimeNode RootNode;

        public bool IsDisposed;

        public long RuntimeId;

        public BTPackage Package;

        public BTDefinition Tree;

        public BTBlackboard Blackboard;

        public BTExecutionContext Context;
    }
}
