using System;

namespace ET
{
    public static partial class BTRuntimeNodeSystem
    {
        public static void Start(this BTRunner self)
        {
            if (self == null || self.IsDisposed)
            {
                return;
            }

            self.RootNode?.Start();
        }

        public static void Stop(this BTRunner self)
        {
            if (self == null || self.IsDisposed)
            {
                return;
            }

            self.RootNode?.Stop();
            PublishDebug(self);
        }

        public static void Dispose(this BTRunner self)
        {
            if (self == null || self.IsDisposed)
            {
                return;
            }

            self.Stop();
            self.IsDisposed = true;
            BTDebugHub.Instance.Remove(self.RuntimeId);
        }

        public static BTDefinition ResolveTree(this BTRunner self, string treeId, string treeName)
        {
            if (!string.IsNullOrWhiteSpace(treeId) && self.TreeIdMap.TryGetValue(treeId, out BTDefinition treeById))
            {
                return treeById;
            }

            if (!string.IsNullOrWhiteSpace(treeName) && self.TreeNameMap.TryGetValue(treeName, out BTDefinition treeByName))
            {
                return treeByName;
            }

            return null;
        }

        public static BTRuntimeNode CreateSubTree(this BTRunner self, string treeId, string treeName, BTRuntimeNode parent)
        {
            BTDefinition definition = self.ResolveTree(treeId, treeName);
            return definition == null ? null : self.BuildTree(definition, definition.RootNodeId, parent);
        }

        public static async ETTask WaitAsync(this BTRunner self, int milliseconds, ETCancellationToken cancellationToken)
        {
            Entity owner = self.Owner;
            TimerComponent timerComponent = owner?.Root()?.GetComponent<TimerComponent>();
            if (timerComponent == null)
            {
                return;
            }

            await timerComponent.WaitAsync(milliseconds, cancellationToken);
        }

        public static void RecordState(this BTRunner self, BTRuntimeNode node, BTNodeState state)
        {
            self.NodeStates[node.NodeId] = state;
            PublishDebug(self);
        }

        public static void LogException(this BTRunner self, Exception exception, BTNodeData node)
        {
            Log.Error($"behavior tree runtime exception: tree={self.Tree?.TreeName} node={node?.Title} id={node?.NodeId}\n{exception}");
        }

        public static void PublishDebug(this BTRunner self)
        {
            Entity owner = self.Owner;
            BTDebugHub.Instance.Publish(self.RuntimeId, self.Tree.TreeId, self.Tree.TreeName, owner?.InstanceId ?? 0, self.NodeStates);
        }

        public static BTRuntimeNode BuildTree(this BTRunner self, BTDefinition tree, string nodeId, BTRuntimeNode parent)
        {
            BTNodeData definition = tree.GetNode(nodeId);
            if (definition == null)
            {
                return null;
            }

            BTRuntimeNode runtimeNode = definition.NodeKind switch
            {
                BTNodeKind.Root => new BTRootNode(self, definition, parent),
                BTNodeKind.Sequence => new BTSequenceNode(self, definition, parent),
                BTNodeKind.Selector => new BTSelectorNode(self, definition, parent),
                BTNodeKind.Parallel => new BTParallelNode(self, definition, parent),
                BTNodeKind.Inverter => new BTInverterNode(self, definition, parent),
                BTNodeKind.Succeeder => new BTSucceederNode(self, definition, parent),
                BTNodeKind.Failer => new BTFailerNode(self, definition, parent),
                BTNodeKind.Repeater => new BTRepeaterNode(self, definition, parent),
                BTNodeKind.BlackboardCondition => new BTBlackboardConditionNode(self, definition, parent),
                BTNodeKind.Service => new BTServiceNode(self, definition, parent),
                BTNodeKind.Action => new BTActionNode(self, definition, parent),
                BTNodeKind.Condition => new BTConditionNode(self, definition, parent),
                BTNodeKind.Wait => new BTWaitNode(self, definition, parent),
                BTNodeKind.SubTree => new BTSubTreeNode(self, definition, parent),
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
