using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public sealed class BehaviorTreeRunner
    {
        public EntityRef<Entity> Owner;

        public Dictionary<string, BehaviorTreeDefinition> TreeIdMap = new();

        public Dictionary<string, BehaviorTreeDefinition> TreeNameMap = new(System.StringComparer.OrdinalIgnoreCase);

        public Dictionary<string, BehaviorTreeNodeState> NodeStates = new();

        public BTRuntimeNode RootNode;

        public bool IsDisposed;

        public long RuntimeId;

        public BehaviorTreePackage Package;

        public BehaviorTreeDefinition Tree;

        public BehaviorTreeBlackboard Blackboard;

        public BehaviorTreeExecutionContext Context;
    }
}
