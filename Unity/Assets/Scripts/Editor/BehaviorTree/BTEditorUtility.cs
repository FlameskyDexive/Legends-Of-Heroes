using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace ET
{
    public static class BTEditorUtility
    {
        private static string[] actionHandlerNames;
        private static string[] conditionHandlerNames;
        private static string[] serviceHandlerNames;
        private static Dictionary<string, ABTNodeDescriptor> nodeDescriptors;
        private static Dictionary<string, ABTNodeDescriptor> handlerNodeDescriptors;
        private static List<ABTNodeDescriptor> orderedNodeDescriptors;

        public static string GetDefaultTitle(BTNodeKind nodeKind, string nodeTypeId = "")
        {
            if (TryGetDescriptorByTypeId(nodeTypeId, out ABTNodeDescriptor descriptor))
            {
                return descriptor.Title;
            }

            return GetLegacyDefaultTitle(nodeKind);
        }

        public static string GetNodeTitle(BTEditorNodeData node)
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

        public static string GetNodeScriptName(BTEditorNodeData node)
        {
            if (TryGetDescriptor(node, out ABTNodeDescriptor descriptor))
            {
                return descriptor.Title;
            }

            return node?.NodeKind.ToString() ?? string.Empty;
        }

        public static bool CanDelete(BTEditorNodeData node)
        {
            return node != null && node.NodeKind != BTNodeKind.Root;
        }

        public static bool HasInputPort(BTNodeKind nodeKind)
        {
            return nodeKind != BTNodeKind.Root;
        }

        public static bool HasOutputPort(BTNodeKind nodeKind)
        {
            return nodeKind is not (BTNodeKind.Action or BTNodeKind.Condition or BTNodeKind.Wait or BTNodeKind.SubTree);
        }

        public static Port.Capacity GetOutputCapacity(BTNodeKind nodeKind)
        {
            return nodeKind is BTNodeKind.Root or BTNodeKind.Inverter or BTNodeKind.Succeeder or BTNodeKind.Failer or BTNodeKind.Repeater or BTNodeKind.BlackboardCondition or BTNodeKind.Service
                    ? Port.Capacity.Single
                    : Port.Capacity.Multi;
        }

        public static string[] GetActionHandlerNames()
        {
            return actionHandlerNames ??= GetHandlerNames<BTActionHandlerAttribute>();
        }

        public static string[] GetConditionHandlerNames()
        {
            return conditionHandlerNames ??= GetHandlerNames<BTConditionHandlerAttribute>();
        }

        public static string[] GetServiceHandlerNames()
        {
            return serviceHandlerNames ??= GetHandlerNames<BTServiceHandlerAttribute>();
        }

        public static IReadOnlyList<ABTNodeDescriptor> GetAllNodeDescriptors()
        {
            EnsureDescriptorCaches();
            return orderedNodeDescriptors;
        }

        public static IReadOnlyList<ABTNodeDescriptor> GetNodeDescriptors(BTNodeKind nodeKind)
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

        public static Color GetNodeColor(BTNodeState state)
        {
            return state switch
            {
                BTNodeState.Running => new Color(0.22f, 0.51f, 0.95f),
                BTNodeState.Success => new Color(0.20f, 0.70f, 0.30f),
                BTNodeState.Failure => new Color(0.82f, 0.25f, 0.25f),
                BTNodeState.Aborted => new Color(0.90f, 0.58f, 0.12f),
                _ => new Color(0.24f, 0.24f, 0.24f),
            };
        }

        public static Color GetNodeHeaderColor(BTNodeKind nodeKind, BTNodeState state)
        {
            if (state != BTNodeState.Inactive)
            {
                return GetNodeColor(state);
            }

            return nodeKind switch
            {
                BTNodeKind.Root => new Color(0.93f, 0.28f, 0.30f),
                BTNodeKind.Sequence or BTNodeKind.Selector or BTNodeKind.Parallel => new Color(0.92f, 0.67f, 0.16f),
                BTNodeKind.Inverter or BTNodeKind.Succeeder or BTNodeKind.Failer or BTNodeKind.Repeater or BTNodeKind.BlackboardCondition or BTNodeKind.Service or BTNodeKind.SubTree => new Color(0.23f, 0.56f, 0.95f),
                BTNodeKind.Action or BTNodeKind.Condition or BTNodeKind.Wait => new Color(0.48f, 0.78f, 0.10f),
                _ => new Color(0.24f, 0.24f, 0.24f),
            };
        }

        public static string GetNodeSummary(BTEditorNodeData node)
        {
            if (node == null)
            {
                return string.Empty;
            }

            SyncNodeDescriptor(node);
            if (TryGetDescriptor(node, out ABTNodeDescriptor descriptor))
            {
                string header = node.NodeKind switch
                {
                    BTNodeKind.Action => $"Behavior: {descriptor.Title}",
                    BTNodeKind.Condition => $"Condition: {descriptor.Title}",
                    BTNodeKind.Service => $"Service: {descriptor.Title}",
                    _ => descriptor.Title,
                };

                List<string> details = new();
                if (node.NodeKind == BTNodeKind.Service)
                {
                    details.Add($"Interval: {node.IntervalMilliseconds}ms");
                }

                if (string.Equals(node.NodeTypeId, ET.BTPatrolNodeTypes.Patrol, StringComparison.OrdinalIgnoreCase))
                {
                    details.Add($"Patrol Points: {node.PatrolPoints.Count}");
                }

                foreach (BTNodeParameterDefinition parameter in descriptor.Parameters.Take(2))
                {
                    if (!TryGetArgument(node, parameter.Name, out BTArgumentData argument) || argument.Value == null)
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
                BTNodeKind.Action when string.Equals(node.NodeTypeId, ET.BTPatrolNodeTypes.Patrol, StringComparison.OrdinalIgnoreCase) => $"Patrol Points: {node.PatrolPoints.Count}",
                BTNodeKind.Action => $"Handler: {node.HandlerName}",
                BTNodeKind.Condition => $"Handler: {node.HandlerName}",
                BTNodeKind.Service => $"Service: {node.HandlerName}\nInterval: {node.IntervalMilliseconds}ms",
                BTNodeKind.Wait => $"Delay: {node.WaitMilliseconds}ms",
                BTNodeKind.Repeater => $"Loop: {(node.MaxLoopCount <= 0 ? "Infinite" : node.MaxLoopCount.ToString())}",
                BTNodeKind.BlackboardCondition => $"Key: {node.BlackboardKey}\nOp: {node.CompareOperator}",
                BTNodeKind.SubTree => $"SubTree: {node.SubTreeName}",
                BTNodeKind.Parallel => $"Success: {node.SuccessPolicy}\nFailure: {node.FailurePolicy}",
                _ => node.Comment,
            };
        }

        public static bool TryGetDescriptor(BTEditorNodeData node, out ABTNodeDescriptor descriptor)
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

        public static void SyncNodeDescriptor(BTEditorNodeData node, bool forceTitle = false)
        {
            if (!TryGetDescriptor(node, out ABTNodeDescriptor descriptor))
            {
                return;
            }

            string previousHandlerName = node.HandlerName;
            string previousDefaultTitle = GetLegacyDefaultTitle(node.NodeKind);
            node.NodeTypeId = descriptor.TypeId;
            node.NodeKind = descriptor.NodeKind;
            node.HandlerName = descriptor.HandlerName;
            node.Arguments ??= new List<BTArgumentData>();

            if (forceTitle || string.IsNullOrWhiteSpace(node.Title) || string.Equals(node.Title, previousDefaultTitle, StringComparison.OrdinalIgnoreCase) || string.Equals(node.Title, previousHandlerName, StringComparison.OrdinalIgnoreCase))
            {
                node.Title = descriptor.Title;
            }

            SyncArguments(node.Arguments, descriptor.Parameters);
        }

        public static bool TryGetArgument(BTEditorNodeData node, string argumentName, out BTArgumentData argument)
        {
            argument = null;
            if (node == null || string.IsNullOrWhiteSpace(argumentName) || node.Arguments == null)
            {
                return false;
            }

            foreach (BTArgumentData currentArgument in node.Arguments)
            {
                if (currentArgument != null && string.Equals(currentArgument.Name, argumentName, StringComparison.OrdinalIgnoreCase))
                {
                    argument = currentArgument;
                    return true;
                }
            }

            return false;
        }

        public static BTArgumentData GetOrCreateArgument(BTEditorNodeData node, BTNodeParameterDefinition parameter)
        {
            node.Arguments ??= new List<BTArgumentData>();
            if (TryGetArgument(node, parameter.Name, out BTArgumentData argument))
            {
                argument.Value ??= parameter.DefaultValue?.Clone() ?? new BTSerializedValue();
                if (parameter.ValueType != BTValueType.None)
                {
                    argument.Value.ValueType = parameter.ValueType;
                }

                return argument;
            }

            argument = new BTArgumentData
            {
                Name = parameter.Name,
                Value = parameter.DefaultValue?.Clone() ?? new BTSerializedValue(),
            };
            if (parameter.ValueType != BTValueType.None)
            {
                argument.Value.ValueType = parameter.ValueType;
            }

            node.Arguments.Add(argument);
            return argument;
        }

        public static bool TryOpenNodeScript(BTEditorNodeData node)
        {
            if (!TryGetNodeScriptAsset(node, out UnityEngine.Object scriptAsset))
            {
                return false;
            }

            AssetDatabase.OpenAsset(scriptAsset);
            return true;
        }

        public static bool TryGetNodeScriptAsset(BTEditorNodeData node, out UnityEngine.Object scriptAsset)
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

        public static bool TryGetNodeHandlerScript(BTEditorNodeData node, out MonoScript monoScript)
        {
            monoScript = null;
            if (!TryGetNodeHandlerType(node, out Type handlerType))
            {
                return false;
            }

            monoScript = FindMonoScript(handlerType);
            return monoScript != null;
        }

        public static bool TryGetNodeHandlerType(BTEditorNodeData node, out Type handlerType)
        {
            handlerType = null;
            if (!TryGetRuntimeNodeType(node, out Type runtimeNodeType))
            {
                return false;
            }

            foreach (Type type in TypeCache.GetTypesWithAttribute<BTNodeHandlerAttribute>())
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

        public static bool TryGetHandlerScript(BTEditorNodeData node, out MonoScript monoScript)
        {
            monoScript = null;
            if (!TryGetHandlerType(node, out Type handlerType))
            {
                return false;
            }

            monoScript = FindMonoScript(handlerType);
            return monoScript != null;
        }

        public static bool TryGetHandlerType(BTEditorNodeData node, out Type handlerType)
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
                BTNodeKind.Action => TryResolveHandlerType<BTActionHandlerAttribute>(node.HandlerName, out handlerType),
                BTNodeKind.Condition => TryResolveHandlerType<BTConditionHandlerAttribute>(node.HandlerName, out handlerType),
                BTNodeKind.Service => TryResolveHandlerType<BTServiceHandlerAttribute>(node.HandlerName, out handlerType),
                _ => false,
            };
        }

        private static string GetLegacyDefaultTitle(BTNodeKind nodeKind)
        {
            return nodeKind switch
            {
                BTNodeKind.Root => "Root",
                BTNodeKind.Sequence => "Sequence",
                BTNodeKind.Selector => "Selector",
                BTNodeKind.Parallel => "Parallel",
                BTNodeKind.Inverter => "Inverter",
                BTNodeKind.Succeeder => "Succeeder",
                BTNodeKind.Failer => "Failer",
                BTNodeKind.Repeater => "Repeater",
                BTNodeKind.BlackboardCondition => "Blackboard Condition",
                BTNodeKind.Service => "Service",
                BTNodeKind.Action => "Action",
                BTNodeKind.Condition => "Condition",
                BTNodeKind.Wait => "Wait",
                BTNodeKind.SubTree => "SubTree",
                _ => nodeKind.ToString(),
            };
        }

        private static string GetArgumentDisplayValue(BTArgumentData argument, BTNodeParameterDefinition parameter)
        {
            if (argument?.Value == null)
            {
                return string.Empty;
            }

            if (parameter.EditorHint == BTNodeParameterEditorHint.CompareOperator)
            {
                return ((BTCompareOperator)BTValueUtility.GetInt(argument.Value, (int)BTCompareOperator.Equal)).ToString();
            }

            return BTValueUtility.ToDisplayString(argument.Value);
        }

        private static void SyncArguments(List<BTArgumentData> arguments, IReadOnlyList<BTNodeParameterDefinition> parameters)
        {
            parameters ??= Array.Empty<BTNodeParameterDefinition>();

            Dictionary<string, BTArgumentData> argumentMap = new(StringComparer.OrdinalIgnoreCase);
            foreach (BTArgumentData argument in arguments)
            {
                if (argument == null || string.IsNullOrWhiteSpace(argument.Name) || argumentMap.ContainsKey(argument.Name))
                {
                    continue;
                }

                argumentMap.Add(argument.Name, argument);
            }

            arguments.Clear();
            foreach (BTNodeParameterDefinition parameter in parameters)
            {
                if (parameter == null || string.IsNullOrWhiteSpace(parameter.Name))
                {
                    continue;
                }

                BTArgumentData argument = argumentMap.TryGetValue(parameter.Name, out BTArgumentData currentArgument)
                        ? currentArgument.Clone()
                        : new BTArgumentData
                        {
                            Name = parameter.Name,
                            Value = parameter.DefaultValue?.Clone() ?? new BTSerializedValue(),
                        };

                argument.Name = parameter.Name;
                argument.Value ??= parameter.DefaultValue?.Clone() ?? new BTSerializedValue();
                if (argument.Value.ValueType == BTValueType.None && parameter.DefaultValue != null)
                {
                    argument.Value = parameter.DefaultValue.Clone();
                }

                if (parameter.ValueType != BTValueType.None)
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

            nodeDescriptors = new Dictionary<string, ABTNodeDescriptor>(StringComparer.OrdinalIgnoreCase);
            handlerNodeDescriptors = new Dictionary<string, ABTNodeDescriptor>(StringComparer.OrdinalIgnoreCase);
            orderedNodeDescriptors = new List<ABTNodeDescriptor>();

            foreach (Type type in TypeCache.GetTypesWithAttribute<BTNodeDescriptorAttribute>())
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                if (Activator.CreateInstance(type) is not ABTNodeDescriptor descriptor)
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

        private static bool TryGetDescriptorByTypeId(string nodeTypeId, out ABTNodeDescriptor descriptor)
        {
            EnsureDescriptorCaches();
            descriptor = null;
            return !string.IsNullOrWhiteSpace(nodeTypeId) && nodeDescriptors.TryGetValue(nodeTypeId, out descriptor);
        }

        private static bool TryGetDescriptorByHandler(BTNodeKind nodeKind, string handlerName, out ABTNodeDescriptor descriptor)
        {
            EnsureDescriptorCaches();
            descriptor = null;
            return !string.IsNullOrWhiteSpace(handlerName) && handlerNodeDescriptors.TryGetValue(BuildHandlerKey(nodeKind, handlerName), out descriptor);
        }

        private static string BuildHandlerKey(BTNodeKind nodeKind, string handlerName)
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
            if (Attribute.GetCustomAttribute(type, attributeType) is BTHandlerAttribute handlerAttribute && !string.IsNullOrWhiteSpace(handlerAttribute.Name))
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

        private static bool TryGetRuntimeNodeScript(BTEditorNodeData node, out UnityEngine.Object scriptAsset)
        {
            scriptAsset = null;
            if (node == null)
            {
                return false;
            }

            string assetPath = node.NodeKind switch
            {
                BTNodeKind.Root => "Assets/Scripts/Hotfix/Share/Module/BehaviorTree/BTRootHandler.cs",
                BTNodeKind.Sequence => "Assets/Scripts/Hotfix/Share/Module/BehaviorTree/BTSequenceHandler.cs",
                BTNodeKind.Selector => "Assets/Scripts/Hotfix/Share/Module/BehaviorTree/BTSelectorHandler.cs",
                BTNodeKind.Parallel => "Assets/Scripts/Hotfix/Share/Module/BehaviorTree/BTParallelHandler.cs",
                BTNodeKind.Inverter => "Assets/Scripts/Hotfix/Share/Module/BehaviorTree/BTInverterHandler.cs",
                BTNodeKind.Succeeder => "Assets/Scripts/Hotfix/Share/Module/BehaviorTree/BTSucceederHandler.cs",
                BTNodeKind.Failer => "Assets/Scripts/Hotfix/Share/Module/BehaviorTree/BTFailerHandler.cs",
                BTNodeKind.Repeater => "Assets/Scripts/Hotfix/Share/Module/BehaviorTree/BTRepeaterHandler.cs",
                BTNodeKind.BlackboardCondition => "Assets/Scripts/Hotfix/Share/Module/BehaviorTree/BTBlackboardConditionHandler.cs",
                BTNodeKind.Wait => "Assets/Scripts/Hotfix/Share/Module/BehaviorTree/BTWaitHandler.cs",
                BTNodeKind.SubTree => "Assets/Scripts/Hotfix/Share/Module/BehaviorTree/BTSubTreeCallHandler.cs",
                BTNodeKind.Service => "Assets/Scripts/Hotfix/Share/Module/BehaviorTree/BTServiceCallHandler.cs",
                BTNodeKind.Action => "Assets/Scripts/Hotfix/Share/Module/BehaviorTree/BTActionCallHandler.cs",
                BTNodeKind.Condition => "Assets/Scripts/Hotfix/Share/Module/BehaviorTree/BTConditionCallHandler.cs",
                _ => string.Empty,
            };

            if (string.IsNullOrWhiteSpace(assetPath))
            {
                return false;
            }

            scriptAsset = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
            return scriptAsset != null;
        }

        private static bool TryGetRuntimeNodeType(BTEditorNodeData node, out Type runtimeNodeType)
        {
            runtimeNodeType = null;
            if (node == null)
            {
                return false;
            }

            SyncNodeDescriptor(node);
            runtimeNodeType = node.NodeKind switch
            {
                BTNodeKind.Root => BTEditorRuntimeBridge.ResolveRuntimeType("ET.BTRoot"),
                BTNodeKind.Sequence => BTEditorRuntimeBridge.ResolveRuntimeType("ET.BTSequence"),
                BTNodeKind.Selector => BTEditorRuntimeBridge.ResolveRuntimeType("ET.BTSelector"),
                BTNodeKind.Parallel => BTEditorRuntimeBridge.ResolveRuntimeType("ET.BTParallel"),
                BTNodeKind.Inverter => BTEditorRuntimeBridge.ResolveRuntimeType("ET.BTInverter"),
                BTNodeKind.Succeeder => BTEditorRuntimeBridge.ResolveRuntimeType("ET.BTSucceeder"),
                BTNodeKind.Failer => BTEditorRuntimeBridge.ResolveRuntimeType("ET.BTFailer"),
                BTNodeKind.Repeater => BTEditorRuntimeBridge.ResolveRuntimeType("ET.BTRepeater"),
                BTNodeKind.BlackboardCondition => BTEditorRuntimeBridge.ResolveRuntimeType("ET.BTBlackboardCondition"),
                BTNodeKind.Wait => BTEditorRuntimeBridge.ResolveRuntimeType("ET.BTWait"),
                BTNodeKind.SubTree => BTEditorRuntimeBridge.ResolveRuntimeType("ET.BTSubTreeCall"),
                BTNodeKind.Service => BTEditorRuntimeBridge.ResolveRuntimeType("ET.BTServiceCall"),
                BTNodeKind.Action => GetActionRuntimeNodeType(node.NodeTypeId),
                BTNodeKind.Condition => GetConditionRuntimeNodeType(node.NodeTypeId),
                _ => null,
            };

            return runtimeNodeType != null;
        }

        private static Type GetActionRuntimeNodeType(string nodeTypeId)
        {
            if (string.Equals(nodeTypeId, BTBuiltinNodeTypes.Log, StringComparison.OrdinalIgnoreCase))
            {
                return BTEditorRuntimeBridge.ResolveRuntimeType("ET.BTLog");
            }

            if (string.Equals(nodeTypeId, BTBuiltinNodeTypes.SetBlackboard, StringComparison.OrdinalIgnoreCase))
            {
                return BTEditorRuntimeBridge.ResolveRuntimeType("ET.BTSetBlackboard");
            }

            if (string.Equals(nodeTypeId, BTBuiltinNodeTypes.SetBlackboardIfMissing, StringComparison.OrdinalIgnoreCase))
            {
                return BTEditorRuntimeBridge.ResolveRuntimeType("ET.BTSetBlackboardIfMissing");
            }

            if (string.Equals(nodeTypeId, BTPatrolNodeTypes.Patrol, StringComparison.OrdinalIgnoreCase))
            {
                return BTEditorRuntimeBridge.ResolveRuntimeType("ET.BTPatrol");
            }

            return BTEditorRuntimeBridge.ResolveRuntimeType("ET.BTActionCall");
        }

        private static Type GetConditionRuntimeNodeType(string nodeTypeId)
        {
            if (string.Equals(nodeTypeId, BTBuiltinNodeTypes.BlackboardExists, StringComparison.OrdinalIgnoreCase))
            {
                return BTEditorRuntimeBridge.ResolveRuntimeType("ET.BTBlackboardExists");
            }

            if (string.Equals(nodeTypeId, BTBuiltinNodeTypes.BlackboardCompare, StringComparison.OrdinalIgnoreCase))
            {
                return BTEditorRuntimeBridge.ResolveRuntimeType("ET.BTBlackboardCompare");
            }

            if (string.Equals(nodeTypeId, BTPatrolNodeTypes.HasPatrolPath, StringComparison.OrdinalIgnoreCase))
            {
                return BTEditorRuntimeBridge.ResolveRuntimeType("ET.BTHasPatrolPath");
            }

            return BTEditorRuntimeBridge.ResolveRuntimeType("ET.BTConditionCall");
        }

        private static Type GetNodeHandledType(Type handlerType)
        {
            Type current = handlerType;
            while (current != null)
            {
                if (current.IsGenericType && current.GetGenericTypeDefinition().FullName == "ET.ABTNodeHandler`1")
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

                if (Attribute.GetCustomAttribute(type, typeof(T)) is BTHandlerAttribute handlerAttribute && !string.IsNullOrWhiteSpace(handlerAttribute.Name))
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
