using System;
using System.Collections.Generic;

namespace ET
{
    public static class BehaviorTreeRuntime
    {
        public static BehaviorTreeRunner Create(Entity owner, byte[] bytes, string treeIdOrName = "")
        {
            BehaviorTreePackage package = BehaviorTreeSerializer.Deserialize(bytes);
            return package == null ? null : new BehaviorTreeRunner(owner, package, treeIdOrName);
        }

        public static BehaviorTreeRunner Create(Entity owner, BehaviorTreePackage package, string treeIdOrName = "")
        {
            return package == null ? null : new BehaviorTreeRunner(owner, package, treeIdOrName);
        }
    }

    [EnableClass]
    public sealed class BehaviorTreeRunner : IDisposable
    {
        private readonly EntityRef<Entity> owner;
        private readonly Dictionary<string, BehaviorTreeDefinition> treeIdMap = new();
        private readonly Dictionary<string, BehaviorTreeDefinition> treeNameMap = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, BehaviorTreeNodeState> nodeStates = new();
        private BehaviorTreeRuntimeNode rootNode;
        private bool isDisposed;

        public BehaviorTreeRunner(Entity owner, BehaviorTreePackage package, string treeIdOrName = "")
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            this.owner = owner;
            this.Package = package.Clone();

            foreach (BehaviorTreeDefinition tree in this.Package.Trees)
            {
                if (!string.IsNullOrWhiteSpace(tree.TreeId))
                {
                    this.treeIdMap[tree.TreeId] = tree;
                }

                if (!string.IsNullOrWhiteSpace(tree.TreeName))
                {
                    this.treeNameMap[tree.TreeName] = tree;
                }
            }

            this.Tree = this.ResolveTree(string.IsNullOrWhiteSpace(treeIdOrName) ? this.Package.EntryTreeId : treeIdOrName,
                string.IsNullOrWhiteSpace(treeIdOrName) ? this.Package.EntryTreeName : treeIdOrName);

            if (this.Tree == null)
            {
                throw new Exception($"behavior tree entry not found: {treeIdOrName}");
            }

            this.RuntimeId = IdGenerater.Instance.GenerateInstanceId();
            this.Blackboard = new BehaviorTreeBlackboard(this.Tree.BlackboardEntries);
            this.Context = new BehaviorTreeExecutionContext(this.RuntimeId, this.Tree.TreeId, this.Tree.TreeName, owner, this.Blackboard);
            this.rootNode = this.BuildTree(this.Tree, this.Tree.RootNodeId, null);
            this.PublishDebug();
        }

        public long RuntimeId { get; }

        public BehaviorTreePackage Package { get; }

        public BehaviorTreeDefinition Tree { get; }

        public BehaviorTreeBlackboard Blackboard { get; }

        public BehaviorTreeExecutionContext Context { get; }

        public Entity Owner => this.owner;

        public bool IsDisposed => this.isDisposed;

        internal TimerComponent Timer => this.Owner?.Root()?.GetComponent<TimerComponent>();

        public void Start()
        {
            if (this.isDisposed)
            {
                return;
            }

            this.rootNode?.Start();
        }

        public void Stop()
        {
            if (this.isDisposed)
            {
                return;
            }

            this.rootNode?.Stop();
            this.PublishDebug();
        }

        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            this.Stop();
            this.isDisposed = true;
            BehaviorTreeDebugHub.Instance.Remove(this.RuntimeId);
        }

        internal BehaviorTreeDefinition ResolveTree(string treeId, string treeName)
        {
            if (!string.IsNullOrWhiteSpace(treeId) && this.treeIdMap.TryGetValue(treeId, out BehaviorTreeDefinition treeById))
            {
                return treeById;
            }

            if (!string.IsNullOrWhiteSpace(treeName) && this.treeNameMap.TryGetValue(treeName, out BehaviorTreeDefinition treeByName))
            {
                return treeByName;
            }

            return null;
        }

        internal BehaviorTreeRuntimeNode CreateSubTree(string treeId, string treeName, BehaviorTreeRuntimeNode parent)
        {
            BehaviorTreeDefinition definition = this.ResolveTree(treeId, treeName);
            return definition == null ? null : this.BuildTree(definition, definition.RootNodeId, parent);
        }

        internal async ETTask WaitAsync(int milliseconds, ETCancellationToken cancellationToken)
        {
            TimerComponent timerComponent = this.Timer;
            if (timerComponent == null)
            {
                return;
            }

            await timerComponent.WaitAsync(milliseconds, cancellationToken);
        }

        internal void RecordState(BehaviorTreeRuntimeNode node, BehaviorTreeNodeState state)
        {
            this.nodeStates[node.NodeId] = state;
            this.PublishDebug();
        }

        internal void LogException(Exception exception, BehaviorTreeNodeDefinition node)
        {
            Log.Error($"behavior tree runtime exception: tree={this.Tree?.TreeName} node={node?.Title} id={node?.NodeId}\n{exception}");
        }

        private void PublishDebug()
        {
            BehaviorTreeDebugHub.Instance.Publish(this.RuntimeId, this.Tree.TreeId, this.Tree.TreeName, this.Owner?.InstanceId ?? 0, this.nodeStates);
        }

        private BehaviorTreeRuntimeNode BuildTree(BehaviorTreeDefinition tree, string nodeId, BehaviorTreeRuntimeNode parent)
        {
            BehaviorTreeNodeDefinition definition = tree.GetNode(nodeId);
            if (definition == null)
            {
                return null;
            }

            BehaviorTreeRuntimeNode runtimeNode = definition.NodeKind switch
            {
                BehaviorTreeNodeKind.Root => new BehaviorTreeRootNode(this, definition, parent),
                BehaviorTreeNodeKind.Sequence => new BehaviorTreeSequenceNode(this, definition, parent),
                BehaviorTreeNodeKind.Selector => new BehaviorTreeSelectorNode(this, definition, parent),
                BehaviorTreeNodeKind.Parallel => new BehaviorTreeParallelNode(this, definition, parent),
                BehaviorTreeNodeKind.Inverter => new BehaviorTreeInverterNode(this, definition, parent),
                BehaviorTreeNodeKind.Succeeder => new BehaviorTreeSucceederNode(this, definition, parent),
                BehaviorTreeNodeKind.Failer => new BehaviorTreeFailerNode(this, definition, parent),
                BehaviorTreeNodeKind.Repeater => new BehaviorTreeRepeaterNode(this, definition, parent),
                BehaviorTreeNodeKind.BlackboardCondition => new BehaviorTreeBlackboardConditionNode(this, definition, parent),
                BehaviorTreeNodeKind.Service => new BehaviorTreeServiceNode(this, definition, parent),
                BehaviorTreeNodeKind.Action => new BehaviorTreeActionNode(this, definition, parent),
                BehaviorTreeNodeKind.Condition => new BehaviorTreeConditionNode(this, definition, parent),
                BehaviorTreeNodeKind.Wait => new BehaviorTreeWaitNode(this, definition, parent),
                BehaviorTreeNodeKind.SubTree => new BehaviorTreeSubTreeNode(this, definition, parent),
                _ => null,
            };

            if (runtimeNode == null)
            {
                return null;
            }

            foreach (string childId in definition.ChildIds)
            {
                BehaviorTreeRuntimeNode childNode = this.BuildTree(tree, childId, runtimeNode);
                if (childNode != null)
                {
                    runtimeNode.Children.Add(childNode);
                }
            }

            return runtimeNode;
        }
    }
}
