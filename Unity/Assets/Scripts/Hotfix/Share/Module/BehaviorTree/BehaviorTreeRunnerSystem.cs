using System;

namespace ET
{
    public static partial class BTRuntimeNodeSystem
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

        public static BTRuntimeNode CreateSubTree(this BehaviorTreeRunner self, string treeId, string treeName, BTRuntimeNode parent)
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

        public static void RecordState(this BehaviorTreeRunner self, BTRuntimeNode node, BehaviorTreeNodeState state)
        {
            self.NodeStates[node.NodeId] = state;
            PublishDebug(self);
        }

        public static void LogException(this BehaviorTreeRunner self, Exception exception, BTNodeData node)
        {
            Log.Error($"behavior tree runtime exception: tree={self.Tree?.TreeName} node={node?.Title} id={node?.NodeId}\n{exception}");
        }

        public static void PublishDebug(this BehaviorTreeRunner self)
        {
            Entity owner = self.Owner;
            BehaviorTreeDebugHub.Instance.Publish(self.RuntimeId, self.Tree.TreeId, self.Tree.TreeName, owner?.InstanceId ?? 0, self.NodeStates);
        }

        public static BTRuntimeNode BuildTree(this BehaviorTreeRunner self, BehaviorTreeDefinition tree, string nodeId, BTRuntimeNode parent)
        {
            BTNodeData definition = tree.GetNode(nodeId);
            if (definition == null)
            {
                return null;
            }

            BTRuntimeNode runtimeNode = definition.NodeKind switch
            {
                BehaviorTreeNodeKind.Root => new BTRootNode(self, definition, parent),
                BehaviorTreeNodeKind.Sequence => new BTSequenceNode(self, definition, parent),
                BehaviorTreeNodeKind.Selector => new BTSelectorNode(self, definition, parent),
                BehaviorTreeNodeKind.Parallel => new BTParallelNode(self, definition, parent),
                BehaviorTreeNodeKind.Inverter => new BTInverterNode(self, definition, parent),
                BehaviorTreeNodeKind.Succeeder => new BTSucceederNode(self, definition, parent),
                BehaviorTreeNodeKind.Failer => new BTFailerNode(self, definition, parent),
                BehaviorTreeNodeKind.Repeater => new BTRepeaterNode(self, definition, parent),
                BehaviorTreeNodeKind.BlackboardCondition => new BTBlackboardConditionNode(self, definition, parent),
                BehaviorTreeNodeKind.Service => new BTServiceNode(self, definition, parent),
                BehaviorTreeNodeKind.Action => new BTActionNode(self, definition, parent),
                BehaviorTreeNodeKind.Condition => new BTConditionNode(self, definition, parent),
                BehaviorTreeNodeKind.Wait => new BTWaitNode(self, definition, parent),
                BehaviorTreeNodeKind.SubTree => new BTSubTreeNode(self, definition, parent),
                _ => null,
            };

            if (runtimeNode == null)
            {
                return null;
            }

            foreach (string childId in definition.ChildIds)
            {
                BTRuntimeNode childNode = self.BuildTree(tree, childId, runtimeNode);
                if (childNode != null)
                {
                    runtimeNode.Children.Add(childNode);
                }
            }

            return runtimeNode;
        }
    }
}
