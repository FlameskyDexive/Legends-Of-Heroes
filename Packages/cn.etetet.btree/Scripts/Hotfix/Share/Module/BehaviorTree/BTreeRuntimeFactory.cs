using System;

namespace ET
{
    public static partial class BTreeRuntime
    {
        public static BTreeExecutionSession Create(Entity owner, byte[] bytes, string treeIdOrName = "")
        {
            BTreePackage package = BTreeSerializer.Deserialize(bytes);
            return package == null ? null : Create(owner, package, treeIdOrName);
        }

        public static BTreeExecutionSession Create(Entity owner, BTreePackage package, string treeIdOrName = "")
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            if (package == null)
            {
                return null;
            }

            BTreeExecutionSession session = new()
            {
                Owner = owner,
                Package = package.Clone(),
            };

            foreach (BTreeDefinition tree in session.Package.Trees)
            {
                if (!string.IsNullOrWhiteSpace(tree.TreeId))
                {
                    session.TreeIdMap[tree.TreeId] = tree;
                }

                if (!string.IsNullOrWhiteSpace(tree.TreeName))
                {
                    session.TreeNameMap[tree.TreeName] = tree;
                }
            }

            session.EntryDefinition = session.ResolveTree(string.IsNullOrWhiteSpace(treeIdOrName) ? session.Package.EntryTreeId : treeIdOrName,
                string.IsNullOrWhiteSpace(treeIdOrName) ? session.Package.EntryTreeName : treeIdOrName);
            if (session.EntryDefinition == null)
            {
                throw new Exception($"behavior tree entry not found: {treeIdOrName}");
            }

            session.RuntimeId = IdGenerater.Instance.GenerateId();
            session.Blackboard = new BTreeBlackboard();
            session.Blackboard.ApplyDefaults(session.EntryDefinition.BlackboardEntries);
            session.Env = new BTreeEnv
            {
                Owner = owner,
                RuntimeId = session.RuntimeId,
                Blackboard = session.Blackboard,
                Session = session,
                CurrentTree = session.EntryDefinition,
                TreeId = session.EntryDefinition.TreeId,
                TreeName = session.EntryDefinition.TreeName,
            };
            session.Root = BTreeGraphBuilder.Build(session);
            session.PublishDebug();
            return session;
        }
    }
}
