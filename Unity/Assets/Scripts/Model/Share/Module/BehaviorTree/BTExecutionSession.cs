using System;
using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public sealed class BTExecutionSession
    {
        public long RuntimeId;

        public EntityRef<Entity> Owner;

        public BTPackage Package;

        public BTDefinition EntryDefinition;

        public BTRoot Root;

        public BTEnv Env;

        public BTBlackboard Blackboard;

        public readonly Dictionary<int, BTNode> Nodes = new();

        public readonly Dictionary<string, BTDefinition> TreeIdMap = new();

        public readonly Dictionary<string, BTDefinition> TreeNameMap = new(StringComparer.OrdinalIgnoreCase);

        public readonly Dictionary<int, BTNodeRuntimeState> NodeStates = new();

        public readonly Dictionary<int, BTCoroutineTokenState> CoroutineStates = new();

        public bool IsDisposed;

        public bool IsDispatching;

        public bool PendingRun;

        public bool IsCompleted;
    }

    [EnableClass]
    public sealed class BTNodeRuntimeState
    {
        public BTNodeState State = BTNodeState.Inactive;

        public int CurrentChildIndex;

        public int RepeatCounter;

        public int CompletedCount;

        public int SuccessCount;

        public int FailureCount;

        public long ObserverId;

        public bool IsActive;

        public bool HasForcedResult;

        public BTExecResult ForcedResult;

        public bool ServiceStarted;
    }

    [EnableClass]
    public sealed class BTCoroutineTokenState
    {
        public int RuntimeNodeId;

        public long Version;

        public ETCancellationToken Token;
    }

    [Code]
    public class BTExecutionSessionManager : Singleton<BTExecutionSessionManager>, ISingletonAwake
    {
        public readonly Dictionary<long, BTExecutionSession> Sessions = new();

        public void Awake()
        {
        }

        public void Add(BTExecutionSession session)
        {
            if (session == null || session.RuntimeId == 0)
            {
                return;
            }

            this.Sessions[session.RuntimeId] = session;
        }

        public BTExecutionSession Get(long runtimeId)
        {
            return runtimeId != 0 && this.Sessions.TryGetValue(runtimeId, out BTExecutionSession session) ? session : null;
        }

        public BTExecutionSession Remove(long runtimeId)
        {
            if (runtimeId == 0)
            {
                return null;
            }

            this.Sessions.Remove(runtimeId, out BTExecutionSession session);
            return session;
        }
    }

    public static class BTExecutionSessionSystem
    {
        public static BTExecutionSession GetSession(this BTEnv self)
        {
            return self?.Session;
        }

        public static BTNodeRuntimeState GetState(this BTEnv self, BTNode node)
        {
            if (self == null || node == null)
            {
                return null;
            }

            BTExecutionSession session = self.GetSession();
            if (session != null)
            {
                if (!session.NodeStates.TryGetValue(node.RuntimeNodeId, out BTNodeRuntimeState state))
                {
                    state = new BTNodeRuntimeState();
                    session.NodeStates[node.RuntimeNodeId] = state;
                }

                return state;
            }

            if (!self.States.TryGetValue(node.RuntimeNodeId, out BTNodeRuntimeState runtimeState))
            {
                runtimeState = new BTNodeRuntimeState();
                self.States[node.RuntimeNodeId] = runtimeState;
            }

            return runtimeState;
        }

        public static bool TryGetState(this BTEnv self, BTNode node, out BTNodeRuntimeState state)
        {
            state = null;
            if (self == null || node == null)
            {
                return false;
            }

            BTExecutionSession session = self.GetSession();
            if (session != null)
            {
                return session.NodeStates.TryGetValue(node.RuntimeNodeId, out state);
            }

            return self.States.TryGetValue(node.RuntimeNodeId, out state);
        }

        public static void RemoveState(this BTEnv self, BTNode node)
        {
            if (self == null || node == null)
            {
                return;
            }

            BTExecutionSession session = self.GetSession();
            session?.NodeStates.Remove(node.RuntimeNodeId);
            self.States.Remove(node.RuntimeNodeId);
        }

        public static BTDefinition ResolveTree(this BTExecutionSession self, string treeId, string treeName)
        {
            if (self == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(treeId) && self.TreeIdMap.TryGetValue(treeId, out BTDefinition definitionById))
            {
                return definitionById;
            }

            if (!string.IsNullOrWhiteSpace(treeName) && self.TreeNameMap.TryGetValue(treeName, out BTDefinition definitionByName))
            {
                return definitionByName;
            }

            return null;
        }

        public static void SetState(this BTExecutionSession self, BTNode node, BTNodeState state)
        {
            if (self == null || node == null)
            {
                return;
            }

            BTNodeRuntimeState runtimeState = self.Env.GetState(node);
            runtimeState.State = state;
            runtimeState.IsActive = state == BTNodeState.Running;
            self.PublishDebug();
        }

        public static void UpdateTreeContext(this BTExecutionSession self, BTNode node)
        {
            if (self == null || node == null)
            {
                return;
            }

            self.Env.TreeId = node.TreeId ?? string.Empty;
            self.Env.TreeName = node.TreeName ?? string.Empty;
            self.Env.CurrentTree = self.ResolveTree(node.TreeId, node.TreeName);
        }

        public static void PublishDebug(this BTExecutionSession self)
        {
            if (self == null)
            {
                return;
            }

            Dictionary<string, BTNodeState> nodeStates = new(StringComparer.OrdinalIgnoreCase);
            foreach ((int runtimeNodeId, BTNodeRuntimeState runtimeState) in self.NodeStates)
            {
                if (!self.Nodes.TryGetValue(runtimeNodeId, out BTNode node) || node == null)
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
            BTDebugHub.Instance.Snapshots[self.RuntimeId] = new BTDebugSnapshot
            {
                RuntimeId = self.RuntimeId,
                TreeId = self.EntryDefinition?.TreeId ?? string.Empty,
                TreeName = self.EntryDefinition?.TreeName ?? string.Empty,
                OwnerInstanceId = owner?.InstanceId ?? 0,
                UpdatedAt = TimeInfo.Instance.ServerNow(),
                NodeStates = nodeStates,
                BlackboardValues = blackboardValues,
            };
        }

        public static void LogException(this BTExecutionSession self, Exception exception, BTNode node)
        {
            Log.Error($"behavior tree runtime exception: tree={node?.TreeName} node={node?.Definition?.Title} id={node?.SourceNodeId}\n{exception}");
        }

        public static BTNodeState ToNodeState(this BTExecResult result)
        {
            return result switch
            {
                BTExecResult.Success => BTNodeState.Success,
                BTExecResult.Failure => BTNodeState.Failure,
                _ => BTNodeState.Running,
            };
        }

        public static BTExecResult ToExecResult(this BTNodeState state)
        {
            return state switch
            {
                BTNodeState.Success => BTExecResult.Success,
                BTNodeState.Running => BTExecResult.Running,
                _ => BTExecResult.Failure,
            };
        }
    }
}
