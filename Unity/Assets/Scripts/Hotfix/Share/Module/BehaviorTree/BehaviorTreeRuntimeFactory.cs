using System;

namespace ET
{
    public static partial class BehaviorTreeRuntime
    {
        public static BehaviorTreeRunner Create(Entity owner, byte[] bytes, string treeIdOrName = "")
        {
            BehaviorTreePackage package = BehaviorTreeSerializer.Deserialize(bytes);
            return package == null ? null : Create(owner, package, treeIdOrName);
        }

        public static BehaviorTreeRunner Create(Entity owner, BehaviorTreePackage package, string treeIdOrName = "")
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            if (package == null)
            {
                return null;
            }

            BehaviorTreeRunner runner = new()
            {
                Owner = owner,
                Package = package.Clone(),
            };

            foreach (BehaviorTreeDefinition tree in runner.Package.Trees)
            {
                if (!string.IsNullOrWhiteSpace(tree.TreeId))
                {
                    runner.TreeIdMap[tree.TreeId] = tree;
                }

                if (!string.IsNullOrWhiteSpace(tree.TreeName))
                {
                    runner.TreeNameMap[tree.TreeName] = tree;
                }
            }

            runner.Tree = runner.ResolveTree(string.IsNullOrWhiteSpace(treeIdOrName) ? runner.Package.EntryTreeId : treeIdOrName,
                string.IsNullOrWhiteSpace(treeIdOrName) ? runner.Package.EntryTreeName : treeIdOrName);
            if (runner.Tree == null)
            {
                throw new Exception($"behavior tree entry not found: {treeIdOrName}");
            }

            runner.RuntimeId = IdGenerater.Instance.GenerateInstanceId();
            runner.Blackboard = new BehaviorTreeBlackboard();
            runner.Blackboard.ApplyDefaults(runner.Tree.BlackboardEntries);
            runner.Context = new BehaviorTreeExecutionContext(runner.RuntimeId, runner.Tree.TreeId, runner.Tree.TreeName, owner, runner.Blackboard);
            runner.RootNode = runner.BuildTree(runner.Tree, runner.Tree.RootNodeId, null);
            runner.PublishDebug();
            return runner;
        }
    }
}
