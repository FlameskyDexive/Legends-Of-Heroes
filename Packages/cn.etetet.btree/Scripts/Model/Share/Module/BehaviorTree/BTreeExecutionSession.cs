using System;
using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public sealed class BTreeExecutionSession
    {
        public long RuntimeId;

        public EntityRef<Entity> Owner;

        public BTreePackage Package;

        public BTreeDefinition EntryDefinition;

        public BTreeRoot Root;

        public BTreeEnv Env;

        public BTreeBlackboard Blackboard;

        public readonly Dictionary<int, BTreeNode> Nodes = new();

        public readonly Dictionary<string, BTreeDefinition> TreeIdMap = new();

        public readonly Dictionary<string, BTreeDefinition> TreeNameMap = new(StringComparer.OrdinalIgnoreCase);

        public readonly Dictionary<int, BTreeNodeRuntimeState> NodeStates = new();

        public readonly Dictionary<int, BTreeCoroutineTokenState> CoroutineStates = new();

        public bool IsDisposed;

        public bool IsDispatching;

        public bool PendingRun;

        public bool IsCompleted;
    }

    [EnableClass]
    public sealed class BTreeNodeRuntimeState
    {
        public BTreeNodeState State = BTreeNodeState.Inactive;

        public int CurrentChildIndex;

        public int RepeatCounter;

        public int CompletedCount;

        public int SuccessCount;

        public int FailureCount;

        public long ObserverId;

        public bool IsActive;

        public bool HasForcedResult;

        public BTreeExecResult ForcedResult;

        public bool ServiceStarted;
    }

    [EnableClass]
    public sealed class BTreeCoroutineTokenState
    {
        public int RuntimeNodeId;

        public long Version;

        public ETCancellationToken Token;
    }

    [CodeProcess]
    [AllowInstance]
    public class BTreeExecutionSessionManager : Singleton<BTreeExecutionSessionManager>, ISingletonAwake
    {
        public readonly Dictionary<long, BTreeExecutionSession> Sessions = new();

        public void Awake()
        {
        }

        public void Add(BTreeExecutionSession session)
        {
            if (session == null || session.RuntimeId == 0)
            {
                return;
            }

            this.Sessions[session.RuntimeId] = session;
        }

        public BTreeExecutionSession Get(long runtimeId)
        {
            return runtimeId != 0 && this.Sessions.TryGetValue(runtimeId, out BTreeExecutionSession session) ? session : null;
        }

        public BTreeExecutionSession Remove(long runtimeId)
        {
            if (runtimeId == 0)
            {
                return null;
            }

            this.Sessions.Remove(runtimeId, out BTreeExecutionSession session);
            return session;
        }
    }

    public static class BTreeExecutionSessionSystem
    {
        public static BTreeExecutionSession GetSession(this BTreeEnv self)
        {
            return self?.Session;
        }

        public static BTreeNodeRuntimeState GetState(this BTreeEnv self, BTreeNode node)
        {
            if (self == null || node == null)
            {
                return null;
            }

            BTreeExecutionSession session = self.GetSession();
            if (session != null)
            {
                if (!session.NodeStates.TryGetValue(node.RuntimeNodeId, out BTreeNodeRuntimeState state))
                {
                    state = new BTreeNodeRuntimeState();
                    session.NodeStates[node.RuntimeNodeId] = state;
                }

                return state;
            }

            if (!self.States.TryGetValue(node.RuntimeNodeId, out BTreeNodeRuntimeState runtimeState))
            {
                runtimeState = new BTreeNodeRuntimeState();
                self.States[node.RuntimeNodeId] = runtimeState;
            }

            return runtimeState;
        }

        public static bool TryGetState(this BTreeEnv self, BTreeNode node, out BTreeNodeRuntimeState state)
        {
            state = null;
            if (self == null || node == null)
            {
                return false;
            }

            BTreeExecutionSession session = self.GetSession();
            if (session != null)
            {
                return session.NodeStates.TryGetValue(node.RuntimeNodeId, out state);
            }

            return self.States.TryGetValue(node.RuntimeNodeId, out state);
        }

        public static void RemoveState(this BTreeEnv self, BTreeNode node)
        {
            if (self == null || node == null)
            {
                return;
            }

            BTreeExecutionSession session = self.GetSession();
            session?.NodeStates.Remove(node.RuntimeNodeId);
            self.States.Remove(node.RuntimeNodeId);
        }

        public static BTreeDefinition ResolveTree(this BTreeExecutionSession self, string treeId, string treeName)
        {
            if (self == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(treeId) && self.TreeIdMap.TryGetValue(treeId, out BTreeDefinition definitionById))
            {
                return definitionById;
            }

            if (!string.IsNullOrWhiteSpace(treeName) && self.TreeNameMap.TryGetValue(treeName, out BTreeDefinition definitionByName))
            {
                return definitionByName;
            }

            return null;
        }

        public static void SetState(this BTreeExecutionSession self, BTreeNode node, BTreeNodeState state)
        {
            if (self == null || node == null)
            {
                return;
            }

            BTreeNodeRuntimeState runtimeState = self.Env.GetState(node);
            runtimeState.State = state;
            runtimeState.IsActive = state == BTreeNodeState.Running;
            self.PublishDebug();
        }

        public static void UpdateTreeContext(this BTreeExecutionSession self, BTreeNode node)
        {
            if (self == null || node == null)
            {
                return;
            }

            self.Env.TreeId = node.TreeId ?? string.Empty;
            self.Env.TreeName = node.TreeName ?? string.Empty;
            self.Env.CurrentTree = self.ResolveTree(node.TreeId, node.TreeName);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void PublishDebug(this BTreeExecutionSession self)
        {
            if (self == null)
            {
                return;
            }

            Dictionary<string, BTreeNodeState> nodeStates = new(StringComparer.OrdinalIgnoreCase);
            foreach ((int runtimeNodeId, BTreeNodeRuntimeState runtimeState) in self.NodeStates)
            {
                if (!self.Nodes.TryGetValue(runtimeNodeId, out BTreeNode node) || node == null)
                {
                    continue;
                }

                nodeStates[node.SourceNodeId] = runtimeState.State;
            }

            Dictionary<string, string> blackboardValues = new(StringComparer.OrdinalIgnoreCase);
            if (self.Blackboard != null)
            {
                foreach ((string key, object value) in self.Blackboard.Values)
                {
                    blackboardValues[key] = value?.ToString() ?? "null";
                }
            }

            Entity owner = self.Owner;
            long updatedAt = owner != null ? owner.GetSingleton<TimeInfo>().ServerNow() : 0;
            BTreeDebugHub.Instance.Snapshots[self.RuntimeId] = new BTreeDebugSnapshot
            {
                RuntimeId = self.RuntimeId,
                TreeId = self.EntryDefinition?.TreeId ?? string.Empty,
                TreeName = self.EntryDefinition?.TreeName ?? string.Empty,
                OwnerInstanceId = owner?.InstanceId ?? 0,
                UpdatedAt = updatedAt,
                NodeStates = nodeStates,
                BlackboardValues = blackboardValues,
            };
        }

        public static void LogException(this BTreeExecutionSession self, Exception exception, BTreeNode node)
        {
            Log.Error($"behavior tree runtime exception: tree={node?.TreeName} node={node?.Definition?.Title} id={node?.SourceNodeId}\n{exception}");
        }

        public static BTreeNodeState ToNodeState(this BTreeExecResult result)
        {
            return result switch
            {
                BTreeExecResult.Success => BTreeNodeState.Success,
                BTreeExecResult.Failure => BTreeNodeState.Failure,
                _ => BTreeNodeState.Running,
            };
        }

        public static BTreeExecResult ToExecResult(this BTreeNodeState state)
        {
            return state switch
            {
                BTreeNodeState.Success => BTreeExecResult.Success,
                BTreeNodeState.Running => BTreeExecResult.Running,
                _ => BTreeExecResult.Failure,
            };
        }
    }
}
