using System;

namespace ET
{
    public static partial class BTRuntime
    {
        public static BTRunner Create(Entity owner, byte[] bytes, string treeIdOrName = "")
        {
            BTPackage package = BTSerializer.Deserialize(bytes);
            return package == null ? null : Create(owner, package, treeIdOrName);
        }

        public static BTRunner Create(Entity owner, BTPackage package, string treeIdOrName = "")
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            if (package == null)
            {
                return null;
            }

            BTRunner runner = new()
            {
                Owner = owner,
                Package = package.Clone(),
            };

            foreach (BTDefinition tree in runner.Package.Trees)
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
            runner.Blackboard = new BTBlackboard();
            runner.Blackboard.ApplyDefaults(runner.Tree.BlackboardEntries);
            runner.Context = new BTExecutionContext(runner.RuntimeId, runner.Tree.TreeId, runner.Tree.TreeName, owner, runner.Blackboard);
            runner.RootNode = runner.BuildTree(runner.Tree, runner.Tree.RootNodeId, null);
            runner.PublishDebug();
            return runner;
        }
    }
}
