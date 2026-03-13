using System;

namespace ET
{
    public static partial class BehaviorTreeRuntimeNodeSystem
    {
        public static void Start(this BehaviorTreeRunner self)
        {
            if (self == null || self.IsDisposed)
            {
                return;
            }

            self.RootNode?.Start();
        }

        public static void Stop(this BehaviorTreeRunner self)
        {
            if (self == null || self.IsDisposed)
            {
                return;
            }

            self.RootNode?.Stop();
            PublishDebug(self);
        }

        public static void Dispose(this BehaviorTreeRunner self)
        {
            if (self == null || self.IsDisposed)
            {
                return;
            }

            self.Stop();
            self.IsDisposed = true;
            BehaviorTreeDebugHub.Instance.Remove(self.RuntimeId);
        }

        public static BehaviorTreeDefinition ResolveTree(this BehaviorTreeRunner self, string treeId, string treeName)
        {
            if (!string.IsNullOrWhiteSpace(treeId) && self.TreeIdMap.TryGetValue(treeId, out BehaviorTreeDefinition treeById))
            {
                return treeById;
            }

            if (!string.IsNullOrWhiteSpace(treeName) && self.TreeNameMap.TryGetValue(treeName, out BehaviorTreeDefinition treeByName))
            {
                return treeByName;
            }

            return null;
        }

        public static BehaviorTreeRuntimeNode CreateSubTree(this BehaviorTreeRunner self, string treeId, string treeName, BehaviorTreeRuntimeNode parent)
        {
            BehaviorTreeDefinition definition = self.ResolveTree(treeId, treeName);
            return definition == null ? null : self.BuildTree(definition, definition.RootNodeId, parent);
        }

        public static async ETTask WaitAsync(this BehaviorTreeRunner self, int milliseconds, ETCancellationToken cancellationToken)
        {
            Entity owner = self.Owner;
            TimerComponent timerComponent = owner?.Root()?.GetComponent<TimerComponent>();
            if (timerComponent == null)
            {
                return;
            }

            await timerComponent.WaitAsync(milliseconds, cancellationToken);
        }

        public static void RecordState(this BehaviorTreeRunner self, BehaviorTreeRuntimeNode node, BehaviorTreeNodeState state)
        {
            self.NodeStates[node.NodeId] = state;
            PublishDebug(self);
        }

        public static void LogException(this BehaviorTreeRunner self, Exception exception, BehaviorTreeNodeDefinition node)
        {
            Log.Error($"behavior tree runtime exception: tree={self.Tree?.TreeName} node={node?.Title} id={node?.NodeId}\n{exception}");
        }

        public static void PublishDebug(this BehaviorTreeRunner self)
        {
            Entity owner = self.Owner;
            BehaviorTreeDebugHub.Instance.Publish(self.RuntimeId, self.Tree.TreeId, self.Tree.TreeName, owner?.InstanceId ?? 0, self.NodeStates);
        }

        public static BehaviorTreeRuntimeNode BuildTree(this BehaviorTreeRunner self, BehaviorTreeDefinition tree, string nodeId, BehaviorTreeRuntimeNode parent)
        {
            BehaviorTreeNodeDefinition definition = tree.GetNode(nodeId);
            if (definition == null)
            {
                return null;
            }

            BehaviorTreeRuntimeNode runtimeNode = definition.NodeKind switch
            {
                BehaviorTreeNodeKind.Root => new BehaviorTreeRootNode(self, definition, parent),
                BehaviorTreeNodeKind.Sequence => new BehaviorTreeSequenceNode(self, definition, parent),
                BehaviorTreeNodeKind.Selector => new BehaviorTreeSelectorNode(self, definition, parent),
                BehaviorTreeNodeKind.Parallel => new BehaviorTreeParallelNode(self, definition, parent),
                BehaviorTreeNodeKind.Inverter => new BehaviorTreeInverterNode(self, definition, parent),
                BehaviorTreeNodeKind.Succeeder => new BehaviorTreeSucceederNode(self, definition, parent),
                BehaviorTreeNodeKind.Failer => new BehaviorTreeFailerNode(self, definition, parent),
                BehaviorTreeNodeKind.Repeater => new BehaviorTreeRepeaterNode(self, definition, parent),
                BehaviorTreeNodeKind.BlackboardCondition => new BehaviorTreeBlackboardConditionNode(self, definition, parent),
                BehaviorTreeNodeKind.Service => new BehaviorTreeServiceNode(self, definition, parent),
                BehaviorTreeNodeKind.Action => new BehaviorTreeActionNode(self, definition, parent),
                BehaviorTreeNodeKind.Condition => new BehaviorTreeConditionNode(self, definition, parent),
                BehaviorTreeNodeKind.Wait => new BehaviorTreeWaitNode(self, definition, parent),
                BehaviorTreeNodeKind.SubTree => new BehaviorTreeSubTreeNode(self, definition, parent),
                _ => null,
            };

            if (runtimeNode == null)
            {
                return null;
            }

            foreach (string childId in definition.ChildIds)
            {
                BehaviorTreeRuntimeNode childNode = self.BuildTree(tree, childId, runtimeNode);
                if (childNode != null)
                {
                    runtimeNode.Children.Add(childNode);
                }
            }

            return runtimeNode;
        }
    }
}
