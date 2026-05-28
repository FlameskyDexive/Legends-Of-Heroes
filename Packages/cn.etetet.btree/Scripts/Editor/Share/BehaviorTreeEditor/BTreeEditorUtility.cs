using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace ET
{
    public static class BTreeEditorUtility
    {
        private static string[] actionHandlerNames;
        private static string[] conditionHandlerNames;
        private static string[] serviceHandlerNames;
        private static Dictionary<string, ABTreeNodeDescriptor> nodeDescriptors;
        private static Dictionary<string, ABTreeNodeDescriptor> handlerNodeDescriptors;
        private static List<ABTreeNodeDescriptor> orderedNodeDescriptors;

        public static string GetDefaultTitle(BTreeNodeKind nodeKind, string nodeTypeId = "")
        {
            if (TryGetDescriptorByTypeId(nodeTypeId, out ABTreeNodeDescriptor descriptor))
            {
                return descriptor.Title;
            }

            return GetLegacyDefaultTitle(nodeKind);
        }

        public static string GetNodeTitle(BTreeEditorNodeData node)
        {
            if (node == null)
            {
                return string.Empty;
            }

            SyncNodeDescriptor(node);
            return string.IsNullOrWhiteSpace(node.Title)
                    ? GetDefaultTitle(node.NodeKind, node.NodeTypeId)
                    : node.Title;
        }

        public static string GetNodeScriptName(BTreeEditorNodeData node)
        {
            if (TryGetDescriptor(node, out ABTreeNodeDescriptor descriptor))
            {
                return descriptor.Title;
            }

            return node?.NodeKind.ToString() ?? string.Empty;
        }

        public static bool CanDelete(BTreeEditorNodeData node)
        {
            return node != null && node.NodeKind != BTreeNodeKind.Root;
        }

        public static bool HasInputPort(BTreeNodeKind nodeKind)
        {
            return nodeKind != BTreeNodeKind.Root;
        }

        public static bool HasOutputPort(BTreeNodeKind nodeKind)
        {
            return nodeKind is not (BTreeNodeKind.Action or BTreeNodeKind.Condition or BTreeNodeKind.Wait or BTreeNodeKind.SubTree);
        }

        public static Port.Capacity GetOutputCapacity(BTreeNodeKind nodeKind)
        {
            return nodeKind is BTreeNodeKind.Root or BTreeNodeKind.Inverter or BTreeNodeKind.Succeeder or BTreeNodeKind.Failer or BTreeNodeKind.Repeater or BTreeNodeKind.BlackboardCondition or BTreeNodeKind.Service
                    ? Port.Capacity.Single
                    : Port.Capacity.Multi;
        }

        public static string[] GetActionHandlerNames()
        {
            return actionHandlerNames ??= GetHandlerNames<BTreeActionHandlerAttribute>();
        }

        public static string[] GetConditionHandlerNames()
        {
            return conditionHandlerNames ??= GetHandlerNames<BTreeConditionHandlerAttribute>();
        }

        public static string[] GetServiceHandlerNames()
        {
            return serviceHandlerNames ??= GetHandlerNames<BTreeServiceHandlerAttribute>();
        }

        public static IReadOnlyList<ABTreeNodeDescriptor> GetAllNodeDescriptors()
        {
            EnsureDescriptorCaches();
            return orderedNodeDescriptors;
        }

        public static IReadOnlyList<ABTreeNodeDescriptor> GetNodeDescriptors(BTreeNodeKind nodeKind)
        {
            EnsureDescriptorCaches();
            return orderedNodeDescriptors.Where(descriptor => descriptor.NodeKind == nodeKind).ToList();
        }

        public static void InvalidateHandlerCaches()
        {
            actionHandlerNames = null;
            conditionHandlerNames = null;
            serviceHandlerNames = null;
            nodeDescriptors = null;
            handlerNodeDescriptors = null;
            orderedNodeDescriptors = null;
        }

        public static Color GetNodeColor(BTreeNodeState state)
        {
            return state switch
            {
                BTreeNodeState.Running => new Color(0.22f, 0.51f, 0.95f),
                BTreeNodeState.Success => new Color(0.20f, 0.70f, 0.30f),
                BTreeNodeState.Failure => new Color(0.82f, 0.25f, 0.25f),
                BTreeNodeState.Aborted => new Color(0.90f, 0.58f, 0.12f),
                _ => new Color(0.24f, 0.24f, 0.24f),
            };
        }

        public static Color GetNodeHeaderColor(BTreeNodeKind nodeKind, BTreeNodeState state)
        {
            if (state != BTreeNodeState.Inactive)
            {
                return GetNodeColor(state);
            }

            return nodeKind switch
            {
                BTreeNodeKind.Root => new Color(0.93f, 0.28f, 0.30f),
                BTreeNodeKind.Sequence or BTreeNodeKind.Selector or BTreeNodeKind.Parallel => new Color(0.92f, 0.67f, 0.16f),
                BTreeNodeKind.Inverter or BTreeNodeKind.Succeeder or BTreeNodeKind.Failer or BTreeNodeKind.Repeater or BTreeNodeKind.BlackboardCondition or BTreeNodeKind.Service or BTreeNodeKind.SubTree => new Color(0.23f, 0.56f, 0.95f),
                BTreeNodeKind.Action or BTreeNodeKind.Condition or BTreeNodeKind.Wait => new Color(0.48f, 0.78f, 0.10f),
                _ => new Color(0.24f, 0.24f, 0.24f),
            };
        }

        public static string GetNodeSummary(BTreeEditorNodeData node)
        {
            if (node == null)
            {
                return string.Empty;
            }

            SyncNodeDescriptor(node);
            if (TryGetDescriptor(node, out ABTreeNodeDescriptor descriptor))
            {
                string header = node.NodeKind switch
                {
                    BTreeNodeKind.Action => $"Behavior: {descriptor.Title}",
                    BTreeNodeKind.Condition => $"Condition: {descriptor.Title}",
                    BTreeNodeKind.Service => $"Service: {descriptor.Title}",
                    _ => descriptor.Title,
                };

                List<string> details = new();
                if (node.NodeKind == BTreeNodeKind.Service)
                {
                    details.Add($"Interval: {node.IntervalMilliseconds}ms");
                }

                if (string.Equals(node.NodeTypeId, ET.BTreePatrolNodeTypes.Patrol, StringComparison.OrdinalIgnoreCase))
                {
                    details.Add($"Patrol Points: {node.PatrolPoints.Count}");
                }

                foreach (BTreeNodeParameterDefinition parameter in descriptor.Parameters.Take(2))
                {
                    if (!TryGetArgument(node, parameter.Name, out BTreeArgumentData argument) || argument.Value == null)
                    {
                        continue;
                    }

                    string valueText = GetArgumentDisplayValue(argument, parameter);
                    if (string.IsNullOrWhiteSpace(valueText))
                    {
                        continue;
                    }

                    string label = string.IsNullOrWhiteSpace(parameter.DisplayName) ? parameter.Name : parameter.DisplayName;
                    details.Add($"{label}: {valueText}");
                }

                return details.Count == 0 ? header : $"{header}\n{string.Join("\n", details)}";
            }

            return node.NodeKind switch
            {
                BTreeNodeKind.Action when string.Equals(node.NodeTypeId, ET.BTreePatrolNodeTypes.Patrol, StringComparison.OrdinalIgnoreCase) => $"Patrol Points: {node.PatrolPoints.Count}",
                BTreeNodeKind.Action => $"Handler: {node.HandlerName}",
                BTreeNodeKind.Condition => $"Handler: {node.HandlerName}",
                BTreeNodeKind.Service => $"Service: {node.HandlerName}\nInterval: {node.IntervalMilliseconds}ms",
                BTreeNodeKind.Wait => $"Delay: {node.WaitMilliseconds}ms",
                BTreeNodeKind.Repeater => $"Loop: {(node.MaxLoopCount <= 0 ? "Infinite" : node.MaxLoopCount.ToString())}",
                BTreeNodeKind.BlackboardCondition => $"Key: {node.BlackboardKey}\nOp: {node.CompareOperator}",
                BTreeNodeKind.SubTree => $"SubTree: {node.SubTreeName}",
                BTreeNodeKind.Parallel => $"Success: {node.SuccessPolicy}\nFailure: {node.FailurePolicy}",
                _ => node.Comment,
            };
        }

        public static bool TryGetDescriptor(BTreeEditorNodeData node, out ABTreeNodeDescriptor descriptor)
        {
            descriptor = null;
            if (node == null)
            {
                return false;
            }

            if (TryGetDescriptorByTypeId(node.NodeTypeId, out descriptor))
            {
                node.NodeTypeId = descriptor.TypeId;
                return true;
            }

            if (TryGetDescriptorByHandler(node.NodeKind, node.HandlerName, out descriptor))
            {
                node.NodeTypeId = descriptor.TypeId;
                return true;
            }

            return false;
        }

        public static void SyncNodeDescriptor(BTreeEditorNodeData node, bool forceTitle = false)
        {
            if (!TryGetDescriptor(node, out ABTreeNodeDescriptor descriptor))
            {
                return;
            }

            string previousHandlerName = node.HandlerName;
            string previousDefaultTitle = GetLegacyDefaultTitle(node.NodeKind);
            node.NodeTypeId = descriptor.TypeId;
            node.NodeKind = descriptor.NodeKind;
            node.HandlerName = descriptor.HandlerName;
            node.Arguments ??= new List<BTreeArgumentData>();

            if (forceTitle || string.IsNullOrWhiteSpace(node.Title) || string.Equals(node.Title, previousDefaultTitle, StringComparison.OrdinalIgnoreCase) || string.Equals(node.Title, previousHandlerName, StringComparison.OrdinalIgnoreCase))
            {
                node.Title = descriptor.Title;
            }

            SyncArguments(node.Arguments, descriptor.Parameters);
        }

        public static bool TryGetArgument(BTreeEditorNodeData node, string argumentName, out BTreeArgumentData argument)
        {
            argument = null;
            if (node == null || string.IsNullOrWhiteSpace(argumentName) || node.Arguments == null)
            {
                return false;
            }

            foreach (BTreeArgumentData currentArgument in node.Arguments)
            {
                if (currentArgument != null && string.Equals(currentArgument.Name, argumentName, StringComparison.OrdinalIgnoreCase))
                {
                    argument = currentArgument;
                    return true;
                }
            }

            return false;
        }

        public static BTreeArgumentData GetOrCreateArgument(BTreeEditorNodeData node, BTreeNodeParameterDefinition parameter)
        {
            node.Arguments ??= new List<BTreeArgumentData>();
            if (TryGetArgument(node, parameter.Name, out BTreeArgumentData argument))
            {
                argument.Value ??= parameter.DefaultValue?.Clone() ?? new BTreeSerializedValue();
                if (parameter.ValueType != BTreeValueType.None)
                {
                    argument.Value.ValueType = parameter.ValueType;
                }

                return argument;
            }

            argument = new BTreeArgumentData
            {
                Name = parameter.Name,
                Value = parameter.DefaultValue?.Clone() ?? new BTreeSerializedValue(),
            };
            if (parameter.ValueType != BTreeValueType.None)
            {
                argument.Value.ValueType = parameter.ValueType;
            }

            node.Arguments.Add(argument);
            return argument;
        }

        public static bool TryOpenNodeScript(BTreeEditorNodeData node)
        {
            if (!TryGetNodeScriptAsset(node, out UnityEngine.Object scriptAsset))
            {
                return false;
            }

            AssetDatabase.OpenAsset(scriptAsset);
            return true;
        }

        public static bool TryGetNodeScriptAsset(BTreeEditorNodeData node, out UnityEngine.Object scriptAsset)
        {
            scriptAsset = null;
            if (TryGetNodeHandlerScript(node, out MonoScript nodeHandlerScript))
            {
                scriptAsset = nodeHandlerScript;
                return true;
            }

            if (TryGetHandlerScript(node, out MonoScript handlerScript))
            {
                scriptAsset = handlerScript;
                return true;
            }

            return TryGetRuntimeNodeScript(node, out scriptAsset);
        }

        public static bool TryGetNodeHandlerScript(BTreeEditorNodeData node, out MonoScript monoScript)
        {
            monoScript = null;
            if (!TryGetNodeHandlerType(node, out Type handlerType))
            {
                return false;
            }

            monoScript = FindMonoScript(handlerType);
            return monoScript != null;
        }

        public static bool TryGetNodeHandlerType(BTreeEditorNodeData node, out Type handlerType)
        {
            handlerType = null;
            if (!TryGetRuntimeNodeType(node, out Type runtimeNodeType))
            {
                return false;
            }

            foreach (Type type in TypeCache.GetTypesWithAttribute<BTreeNodeHandlerAttribute>())
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                Type currentNodeType = GetNodeHandledType(type);
                if (currentNodeType == null || currentNodeType != runtimeNodeType)
                {
                    continue;
                }

                handlerType = type;
                return true;
            }

            return false;
        }

        public static bool TryGetHandlerScript(BTreeEditorNodeData node, out MonoScript monoScript)
        {
            monoScript = null;
            if (!TryGetHandlerType(node, out Type handlerType))
            {
                return false;
            }

            monoScript = FindMonoScript(handlerType);
            return monoScript != null;
        }

        public static bool TryGetHandlerType(BTreeEditorNodeData node, out Type handlerType)
        {
            handlerType = null;
            if (node == null)
            {
                return false;
            }

            SyncNodeDescriptor(node);
            if (string.IsNullOrWhiteSpace(node.HandlerName))
            {
                return false;
            }

            return node.NodeKind switch
            {
                BTreeNodeKind.Action => TryResolveHandlerType<BTreeActionHandlerAttribute>(node.HandlerName, out handlerType),
                BTreeNodeKind.Condition => TryResolveHandlerType<BTreeConditionHandlerAttribute>(node.HandlerName, out handlerType),
                BTreeNodeKind.Service => TryResolveHandlerType<BTreeServiceHandlerAttribute>(node.HandlerName, out handlerType),
                _ => false,
            };
        }

        private static string GetLegacyDefaultTitle(BTreeNodeKind nodeKind)
        {
            return nodeKind switch
            {
                BTreeNodeKind.Root => "Root",
                BTreeNodeKind.Sequence => "Sequence",
                BTreeNodeKind.Selector => "Selector",
                BTreeNodeKind.Parallel => "Parallel",
                BTreeNodeKind.Inverter => "Inverter",
                BTreeNodeKind.Succeeder => "Succeeder",
                BTreeNodeKind.Failer => "Failer",
                BTreeNodeKind.Repeater => "Repeater",
                BTreeNodeKind.BlackboardCondition => "Blackboard Condition",
                BTreeNodeKind.Service => "Service",
                BTreeNodeKind.Action => "Action",
                BTreeNodeKind.Condition => "Condition",
                BTreeNodeKind.Wait => "Wait",
                BTreeNodeKind.SubTree => "SubTree",
                _ => nodeKind.ToString(),
            };
        }

        private static string GetArgumentDisplayValue(BTreeArgumentData argument, BTreeNodeParameterDefinition parameter)
        {
            if (argument?.Value == null)
            {
                return string.Empty;
            }

            if (parameter.EditorHint == BTreeNodeParameterEditorHint.CompareOperator)
            {
                return ((BTreeCompareOperator)BTreeValueUtility.GetInt(argument.Value, (int)BTreeCompareOperator.Equal)).ToString();
            }

            return BTreeValueUtility.ToDisplayString(argument.Value);
        }

        private static void SyncArguments(List<BTreeArgumentData> arguments, IReadOnlyList<BTreeNodeParameterDefinition> parameters)
        {
            parameters ??= Array.Empty<BTreeNodeParameterDefinition>();

            Dictionary<string, BTreeArgumentData> argumentMap = new(StringComparer.OrdinalIgnoreCase);
            foreach (BTreeArgumentData argument in arguments)
            {
                if (argument == null || string.IsNullOrWhiteSpace(argument.Name) || argumentMap.ContainsKey(argument.Name))
                {
                    continue;
                }

                argumentMap.Add(argument.Name, argument);
            }

            arguments.Clear();
            foreach (BTreeNodeParameterDefinition parameter in parameters)
            {
                if (parameter == null || string.IsNullOrWhiteSpace(parameter.Name))
                {
                    continue;
                }

                BTreeArgumentData argument = argumentMap.TryGetValue(parameter.Name, out BTreeArgumentData currentArgument)
                        ? currentArgument.Clone()
                        : new BTreeArgumentData
                        {
                            Name = parameter.Name,
                            Value = parameter.DefaultValue?.Clone() ?? new BTreeSerializedValue(),
                        };

                argument.Name = parameter.Name;
                argument.Value ??= parameter.DefaultValue?.Clone() ?? new BTreeSerializedValue();
                if (argument.Value.ValueType == BTreeValueType.None && parameter.DefaultValue != null)
                {
                    argument.Value = parameter.DefaultValue.Clone();
                }

                if (parameter.ValueType != BTreeValueType.None)
                {
                    argument.Value.ValueType = parameter.ValueType;
                }

                arguments.Add(argument);
            }
        }

        private static void EnsureDescriptorCaches()
        {
            if (nodeDescriptors != null)
            {
                return;
            }

            nodeDescriptors = new Dictionary<string, ABTreeNodeDescriptor>(StringComparer.OrdinalIgnoreCase);
            handlerNodeDescriptors = new Dictionary<string, ABTreeNodeDescriptor>(StringComparer.OrdinalIgnoreCase);
            orderedNodeDescriptors = new List<ABTreeNodeDescriptor>();

            foreach (Type type in TypeCache.GetTypesWithAttribute<BTreeNodeDescriptorAttribute>())
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                if (Activator.CreateInstance(type) is not ABTreeNodeDescriptor descriptor)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(descriptor.TypeId) || !nodeDescriptors.TryAdd(descriptor.TypeId, descriptor))
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(descriptor.HandlerName))
                {
                    handlerNodeDescriptors.TryAdd(BuildHandlerKey(descriptor.NodeKind, descriptor.HandlerName), descriptor);
                }

                orderedNodeDescriptors.Add(descriptor);
            }

            orderedNodeDescriptors.Sort((left, right) =>
            {
                int orderCompare = left.SortOrder.CompareTo(right.SortOrder);
                if (orderCompare != 0)
                {
                    return orderCompare;
                }

                int menuCompare = string.Compare(left.MenuPath, right.MenuPath, StringComparison.OrdinalIgnoreCase);
                if (menuCompare != 0)
                {
                    return menuCompare;
                }

                return string.Compare(left.Title, right.Title, StringComparison.OrdinalIgnoreCase);
            });
        }

        private static bool TryGetDescriptorByTypeId(string nodeTypeId, out ABTreeNodeDescriptor descriptor)
        {
            EnsureDescriptorCaches();
            descriptor = null;
            return !string.IsNullOrWhiteSpace(nodeTypeId) && nodeDescriptors.TryGetValue(nodeTypeId, out descriptor);
        }

        private static bool TryGetDescriptorByHandler(BTreeNodeKind nodeKind, string handlerName, out ABTreeNodeDescriptor descriptor)
        {
            EnsureDescriptorCaches();
            descriptor = null;
            return !string.IsNullOrWhiteSpace(handlerName) && handlerNodeDescriptors.TryGetValue(BuildHandlerKey(nodeKind, handlerName), out descriptor);
        }

        private static string BuildHandlerKey(BTreeNodeKind nodeKind, string handlerName)
        {
            return $"{(int)nodeKind}:{handlerName}";
        }

        private static bool TryResolveHandlerType<T>(string handlerName, out Type handlerType) where T : Attribute
        {
            handlerType = null;
            if (string.IsNullOrWhiteSpace(handlerName))
            {
                return false;
            }

            foreach (Type type in TypeCache.GetTypesWithAttribute<T>())
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                string currentName = GetHandlerName(type, typeof(T));
                if (!string.Equals(currentName, handlerName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                handlerType = type;
                return true;
            }

            return false;
        }

        private static string GetHandlerName(Type type, Type attributeType)
        {
            if (Attribute.GetCustomAttribute(type, attributeType) is BTreeHandlerAttribute handlerAttribute && !string.IsNullOrWhiteSpace(handlerAttribute.Name))
            {
                return handlerAttribute.Name;
            }

            return type.Name;
        }

        private static MonoScript FindMonoScript(Type type)
        {
            foreach (string guid in AssetDatabase.FindAssets($"{type.Name} t:MonoScript"))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                if (monoScript != null && monoScript.GetClass() == type)
                {
                    return monoScript;
                }
            }

            return null;
        }

        private static bool TryGetRuntimeNodeScript(BTreeEditorNodeData node, out UnityEngine.Object scriptAsset)
        {
            scriptAsset = null;
            if (node == null)
            {
                return false;
            }

            string assetPath = node.NodeKind switch
            {
                BTreeNodeKind.Root => "Assets/Scripts/Hotfix/Share/Module/BehaviorTree/BTreeRootHandler.cs",
                BTreeNodeKind.Sequence => "Assets/Scripts/Hotfix/Share/Module/BehaviorTree/BTreeSequenceHandler.cs",
                BTreeNodeKind.Selector => "Assets/Scripts/Hotfix/Share/Module/BehaviorTree/BTreeSelectorHandler.cs",
                BTreeNodeKind.Parallel => "Assets/Scripts/Hotfix/Share/Module/BehaviorTree/BTreeParallelHandler.cs",
                BTreeNodeKind.Inverter => "Assets/Scripts/Hotfix/Share/Module/BehaviorTree/BTreeInverterHandler.cs",
                BTreeNodeKind.Succeeder => "Assets/Scripts/Hotfix/Share/Module/BehaviorTree/BTreeSucceederHandler.cs",
                BTreeNodeKind.Failer => "Assets/Scripts/Hotfix/Share/Module/BehaviorTree/BTreeFailerHandler.cs",
                BTreeNodeKind.Repeater => "Assets/Scripts/Hotfix/Share/Module/BehaviorTree/BTreeRepeaterHandler.cs",
                BTreeNodeKind.BlackboardCondition => "Assets/Scripts/Hotfix/Share/Module/BehaviorTree/BTreeBlackboardConditionHandler.cs",
                BTreeNodeKind.Wait => "Assets/Scripts/Hotfix/Share/Module/BehaviorTree/BTreeWaitHandler.cs",
                BTreeNodeKind.SubTree => "Assets/Scripts/Hotfix/Share/Module/BehaviorTree/BTreeSubTreeCallHandler.cs",
                BTreeNodeKind.Service => "Assets/Scripts/Hotfix/Share/Module/BehaviorTree/BTreeServiceCallHandler.cs",
                BTreeNodeKind.Action => "Assets/Scripts/Hotfix/Share/Module/BehaviorTree/BTreeActionCallHandler.cs",
                BTreeNodeKind.Condition => "Assets/Scripts/Hotfix/Share/Module/BehaviorTree/BTreeConditionCallHandler.cs",
                _ => string.Empty,
            };

            if (string.IsNullOrWhiteSpace(assetPath))
            {
                return false;
            }

            scriptAsset = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
            return scriptAsset != null;
        }

        private static bool TryGetRuntimeNodeType(BTreeEditorNodeData node, out Type runtimeNodeType)
        {
            runtimeNodeType = null;
            if (node == null)
            {
                return false;
            }

            SyncNodeDescriptor(node);
            runtimeNodeType = node.NodeKind switch
            {
                BTreeNodeKind.Root => BTreeEditorRuntimeBridge.ResolveRuntimeType("ET.BTreeRoot"),
                BTreeNodeKind.Sequence => BTreeEditorRuntimeBridge.ResolveRuntimeType("ET.BTreeSequence"),
                BTreeNodeKind.Selector => BTreeEditorRuntimeBridge.ResolveRuntimeType("ET.BTreeSelector"),
                BTreeNodeKind.Parallel => BTreeEditorRuntimeBridge.ResolveRuntimeType("ET.BTreeParallel"),
                BTreeNodeKind.Inverter => BTreeEditorRuntimeBridge.ResolveRuntimeType("ET.BTreeInverter"),
                BTreeNodeKind.Succeeder => BTreeEditorRuntimeBridge.ResolveRuntimeType("ET.BTreeSucceeder"),
                BTreeNodeKind.Failer => BTreeEditorRuntimeBridge.ResolveRuntimeType("ET.BTreeFailer"),
                BTreeNodeKind.Repeater => BTreeEditorRuntimeBridge.ResolveRuntimeType("ET.BTreeRepeater"),
                BTreeNodeKind.BlackboardCondition => BTreeEditorRuntimeBridge.ResolveRuntimeType("ET.BTreeBlackboardCondition"),
                BTreeNodeKind.Wait => BTreeEditorRuntimeBridge.ResolveRuntimeType("ET.BTreeWait"),
                BTreeNodeKind.SubTree => BTreeEditorRuntimeBridge.ResolveRuntimeType("ET.BTreeSubTreeCall"),
                BTreeNodeKind.Service => BTreeEditorRuntimeBridge.ResolveRuntimeType("ET.BTreeServiceCall"),
                BTreeNodeKind.Action => GetActionRuntimeNodeType(node.NodeTypeId),
                BTreeNodeKind.Condition => GetConditionRuntimeNodeType(node.NodeTypeId),
                _ => null,
            };

            return runtimeNodeType != null;
        }

        private static Type GetActionRuntimeNodeType(string nodeTypeId)
        {
            if (string.Equals(nodeTypeId, BTreeBuiltinNodeTypes.Log, StringComparison.OrdinalIgnoreCase))
            {
                return BTreeEditorRuntimeBridge.ResolveRuntimeType("ET.BTreeLog");
            }

            if (string.Equals(nodeTypeId, BTreeBuiltinNodeTypes.SetBlackboard, StringComparison.OrdinalIgnoreCase))
            {
                return BTreeEditorRuntimeBridge.ResolveRuntimeType("ET.BTreeSetBlackboard");
            }

            if (string.Equals(nodeTypeId, BTreeBuiltinNodeTypes.SetBlackboardIfMissing, StringComparison.OrdinalIgnoreCase))
            {
                return BTreeEditorRuntimeBridge.ResolveRuntimeType("ET.BTreeSetBlackboardIfMissing");
            }

            if (string.Equals(nodeTypeId, BTreePatrolNodeTypes.Patrol, StringComparison.OrdinalIgnoreCase))
            {
                return BTreeEditorRuntimeBridge.ResolveRuntimeType("ET.BTreePatrol");
            }

            return BTreeEditorRuntimeBridge.ResolveRuntimeType("ET.BTreeActionCall");
        }

        private static Type GetConditionRuntimeNodeType(string nodeTypeId)
        {
            if (string.Equals(nodeTypeId, BTreeBuiltinNodeTypes.BlackboardExists, StringComparison.OrdinalIgnoreCase))
            {
                return BTreeEditorRuntimeBridge.ResolveRuntimeType("ET.BTreeBlackboardExists");
            }

            if (string.Equals(nodeTypeId, BTreeBuiltinNodeTypes.BlackboardCompare, StringComparison.OrdinalIgnoreCase))
            {
                return BTreeEditorRuntimeBridge.ResolveRuntimeType("ET.BTreeBlackboardCompare");
            }

            if (string.Equals(nodeTypeId, BTreePatrolNodeTypes.HasPatrolPath, StringComparison.OrdinalIgnoreCase))
            {
                return BTreeEditorRuntimeBridge.ResolveRuntimeType("ET.BTreeHasPatrolPath");
            }

            return BTreeEditorRuntimeBridge.ResolveRuntimeType("ET.BTreeConditionCall");
        }

        private static Type GetNodeHandledType(Type handlerType)
        {
            Type current = handlerType;
            while (current != null)
            {
                if (current.IsGenericType && current.GetGenericTypeDefinition().FullName == "ET.ABTreeNodeHandler`1")
                {
                    return current.GetGenericArguments()[0];
                }

                current = current.BaseType;
            }

            return null;
        }

        private static string[] GetHandlerNames<T>() where T : Attribute
        {
            List<string> names = new();
            foreach (Type type in TypeCache.GetTypesWithAttribute<T>())
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                if (Attribute.GetCustomAttribute(type, typeof(T)) is BTreeHandlerAttribute handlerAttribute && !string.IsNullOrWhiteSpace(handlerAttribute.Name))
                {
                    names.Add(handlerAttribute.Name);
                }
                else
                {
                    names.Add(type.Name);
                }
            }

            names.Sort(StringComparer.OrdinalIgnoreCase);
            return names.ToArray();
        }
    }
}
