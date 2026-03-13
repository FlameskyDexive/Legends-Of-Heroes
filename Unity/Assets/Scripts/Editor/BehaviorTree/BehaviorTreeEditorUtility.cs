using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace ET
{
    public static class BehaviorTreeEditorUtility
    {
        private static string[] actionHandlerNames;
        private static string[] conditionHandlerNames;
        private static string[] serviceHandlerNames;
        private static Dictionary<string, ABehaviorTreeNodeDescriptor> nodeDescriptors;
        private static Dictionary<string, ABehaviorTreeNodeDescriptor> handlerNodeDescriptors;
        private static List<ABehaviorTreeNodeDescriptor> orderedNodeDescriptors;

        public static string GetDefaultTitle(BehaviorTreeNodeKind nodeKind, string nodeTypeId = "")
        {
            if (TryGetDescriptorByTypeId(nodeTypeId, out ABehaviorTreeNodeDescriptor descriptor))
            {
                return descriptor.Title;
            }

            return GetLegacyDefaultTitle(nodeKind);
        }

        public static string GetNodeTitle(BehaviorTreeEditorNodeData node)
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

        public static string GetNodeScriptName(BehaviorTreeEditorNodeData node)
        {
            if (TryGetDescriptor(node, out ABehaviorTreeNodeDescriptor descriptor))
            {
                return descriptor.Title;
            }

            return node?.NodeKind.ToString() ?? string.Empty;
        }

        public static bool CanDelete(BehaviorTreeEditorNodeData node)
        {
            return node != null && node.NodeKind != BehaviorTreeNodeKind.Root;
        }

        public static bool HasInputPort(BehaviorTreeNodeKind nodeKind)
        {
            return nodeKind != BehaviorTreeNodeKind.Root;
        }

        public static bool HasOutputPort(BehaviorTreeNodeKind nodeKind)
        {
            return nodeKind is not (BehaviorTreeNodeKind.Action or BehaviorTreeNodeKind.Condition or BehaviorTreeNodeKind.Wait or BehaviorTreeNodeKind.SubTree);
        }

        public static Port.Capacity GetOutputCapacity(BehaviorTreeNodeKind nodeKind)
        {
            return nodeKind is BehaviorTreeNodeKind.Root or BehaviorTreeNodeKind.Inverter or BehaviorTreeNodeKind.Succeeder or BehaviorTreeNodeKind.Failer or BehaviorTreeNodeKind.Repeater or BehaviorTreeNodeKind.BlackboardCondition or BehaviorTreeNodeKind.Service
                    ? Port.Capacity.Single
                    : Port.Capacity.Multi;
        }

        public static string[] GetActionHandlerNames()
        {
            return actionHandlerNames ??= GetHandlerNames<BehaviorTreeActionHandlerAttribute>();
        }

        public static string[] GetConditionHandlerNames()
        {
            return conditionHandlerNames ??= GetHandlerNames<BehaviorTreeConditionHandlerAttribute>();
        }

        public static string[] GetServiceHandlerNames()
        {
            return serviceHandlerNames ??= GetHandlerNames<BehaviorTreeServiceHandlerAttribute>();
        }

        public static IReadOnlyList<ABehaviorTreeNodeDescriptor> GetAllNodeDescriptors()
        {
            EnsureDescriptorCaches();
            return orderedNodeDescriptors;
        }

        public static IReadOnlyList<ABehaviorTreeNodeDescriptor> GetNodeDescriptors(BehaviorTreeNodeKind nodeKind)
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

        public static Color GetNodeColor(BehaviorTreeNodeState state)
        {
            return state switch
            {
                BehaviorTreeNodeState.Running => new Color(0.22f, 0.51f, 0.95f),
                BehaviorTreeNodeState.Success => new Color(0.20f, 0.70f, 0.30f),
                BehaviorTreeNodeState.Failure => new Color(0.82f, 0.25f, 0.25f),
                BehaviorTreeNodeState.Aborted => new Color(0.90f, 0.58f, 0.12f),
                _ => new Color(0.24f, 0.24f, 0.24f),
            };
        }

        public static Color GetNodeHeaderColor(BehaviorTreeNodeKind nodeKind, BehaviorTreeNodeState state)
        {
            if (state != BehaviorTreeNodeState.Inactive)
            {
                return GetNodeColor(state);
            }

            return nodeKind switch
            {
                BehaviorTreeNodeKind.Root => new Color(0.93f, 0.28f, 0.30f),
                BehaviorTreeNodeKind.Sequence or BehaviorTreeNodeKind.Selector or BehaviorTreeNodeKind.Parallel => new Color(0.92f, 0.67f, 0.16f),
                BehaviorTreeNodeKind.Inverter or BehaviorTreeNodeKind.Succeeder or BehaviorTreeNodeKind.Failer or BehaviorTreeNodeKind.Repeater or BehaviorTreeNodeKind.BlackboardCondition or BehaviorTreeNodeKind.Service or BehaviorTreeNodeKind.SubTree => new Color(0.23f, 0.56f, 0.95f),
                BehaviorTreeNodeKind.Action or BehaviorTreeNodeKind.Condition or BehaviorTreeNodeKind.Wait => new Color(0.48f, 0.78f, 0.10f),
                _ => new Color(0.24f, 0.24f, 0.24f),
            };
        }

        public static string GetNodeSummary(BehaviorTreeEditorNodeData node)
        {
            if (node == null)
            {
                return string.Empty;
            }

            SyncNodeDescriptor(node);
            if (TryGetDescriptor(node, out ABehaviorTreeNodeDescriptor descriptor))
            {
                string header = node.NodeKind switch
                {
                    BehaviorTreeNodeKind.Action => $"Behavior: {descriptor.Title}",
                    BehaviorTreeNodeKind.Condition => $"Condition: {descriptor.Title}",
                    BehaviorTreeNodeKind.Service => $"Service: {descriptor.Title}",
                    _ => descriptor.Title,
                };

                List<string> details = new();
                if (node.NodeKind == BehaviorTreeNodeKind.Service)
                {
                    details.Add($"Interval: {node.IntervalMilliseconds}ms");
                }

                foreach (BehaviorTreeNodeParameterDefinition parameter in descriptor.Parameters.Take(2))
                {
                    if (!TryGetArgument(node, parameter.Name, out BehaviorTreeArgumentDefinition argument) || argument.Value == null)
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
                BehaviorTreeNodeKind.Action => $"Handler: {node.HandlerName}",
                BehaviorTreeNodeKind.Condition => $"Handler: {node.HandlerName}",
                BehaviorTreeNodeKind.Service => $"Service: {node.HandlerName}\nInterval: {node.IntervalMilliseconds}ms",
                BehaviorTreeNodeKind.Wait => $"Delay: {node.WaitMilliseconds}ms",
                BehaviorTreeNodeKind.Repeater => $"Loop: {(node.MaxLoopCount <= 0 ? "∞" : node.MaxLoopCount.ToString())}",
                BehaviorTreeNodeKind.BlackboardCondition => $"Key: {node.BlackboardKey}\nOp: {node.CompareOperator}",
                BehaviorTreeNodeKind.SubTree => $"SubTree: {node.SubTreeName}",
                BehaviorTreeNodeKind.Parallel => $"Success: {node.SuccessPolicy}\nFailure: {node.FailurePolicy}",
                _ => node.Comment,
            };
        }

        public static bool TryGetDescriptor(BehaviorTreeEditorNodeData node, out ABehaviorTreeNodeDescriptor descriptor)
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

        public static void SyncNodeDescriptor(BehaviorTreeEditorNodeData node, bool forceTitle = false)
        {
            if (!TryGetDescriptor(node, out ABehaviorTreeNodeDescriptor descriptor))
            {
                return;
            }

            string previousHandlerName = node.HandlerName;
            string previousDefaultTitle = GetLegacyDefaultTitle(node.NodeKind);
            node.NodeTypeId = descriptor.TypeId;
            node.NodeKind = descriptor.NodeKind;
            node.HandlerName = descriptor.HandlerName;
            node.Arguments ??= new List<BehaviorTreeArgumentDefinition>();

            if (forceTitle || string.IsNullOrWhiteSpace(node.Title) || string.Equals(node.Title, previousDefaultTitle, StringComparison.OrdinalIgnoreCase) || string.Equals(node.Title, previousHandlerName, StringComparison.OrdinalIgnoreCase))
            {
                node.Title = descriptor.Title;
            }

            SyncArguments(node.Arguments, descriptor.Parameters);
        }

        public static bool TryGetArgument(BehaviorTreeEditorNodeData node, string argumentName, out BehaviorTreeArgumentDefinition argument)
        {
            argument = null;
            if (node == null || string.IsNullOrWhiteSpace(argumentName) || node.Arguments == null)
            {
                return false;
            }

            foreach (BehaviorTreeArgumentDefinition currentArgument in node.Arguments)
            {
                if (currentArgument != null && string.Equals(currentArgument.Name, argumentName, StringComparison.OrdinalIgnoreCase))
                {
                    argument = currentArgument;
                    return true;
                }
            }

            return false;
        }

        public static BehaviorTreeArgumentDefinition GetOrCreateArgument(BehaviorTreeEditorNodeData node, BehaviorTreeNodeParameterDefinition parameter)
        {
            node.Arguments ??= new List<BehaviorTreeArgumentDefinition>();
            if (TryGetArgument(node, parameter.Name, out BehaviorTreeArgumentDefinition argument))
            {
                argument.Value ??= parameter.DefaultValue?.Clone() ?? new BehaviorTreeSerializedValue();
                if (parameter.ValueType != BehaviorTreeValueType.None)
                {
                    argument.Value.ValueType = parameter.ValueType;
                }

                return argument;
            }

            argument = new BehaviorTreeArgumentDefinition
            {
                Name = parameter.Name,
                Value = parameter.DefaultValue?.Clone() ?? new BehaviorTreeSerializedValue(),
            };
            if (parameter.ValueType != BehaviorTreeValueType.None)
            {
                argument.Value.ValueType = parameter.ValueType;
            }

            node.Arguments.Add(argument);
            return argument;
        }

        public static bool TryOpenNodeScript(BehaviorTreeEditorNodeData node)
        {
            if (!TryGetNodeScriptAsset(node, out UnityEngine.Object scriptAsset))
            {
                return false;
            }

            AssetDatabase.OpenAsset(scriptAsset);
            return true;
        }

        public static bool TryGetNodeScriptAsset(BehaviorTreeEditorNodeData node, out UnityEngine.Object scriptAsset)
        {
            scriptAsset = null;
            if (TryGetHandlerScript(node, out MonoScript handlerScript))
            {
                scriptAsset = handlerScript;
                return true;
            }

            return TryGetRuntimeNodeScript(node, out scriptAsset);
        }

        public static bool TryGetHandlerScript(BehaviorTreeEditorNodeData node, out MonoScript monoScript)
        {
            monoScript = null;
            if (!TryGetHandlerType(node, out Type handlerType))
            {
                return false;
            }

            monoScript = FindMonoScript(handlerType);
            return monoScript != null;
        }

        public static bool TryGetHandlerType(BehaviorTreeEditorNodeData node, out Type handlerType)
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
                BehaviorTreeNodeKind.Action => TryResolveHandlerType<BehaviorTreeActionHandlerAttribute>(node.HandlerName, out handlerType),
                BehaviorTreeNodeKind.Condition => TryResolveHandlerType<BehaviorTreeConditionHandlerAttribute>(node.HandlerName, out handlerType),
                BehaviorTreeNodeKind.Service => TryResolveHandlerType<BehaviorTreeServiceHandlerAttribute>(node.HandlerName, out handlerType),
                _ => false,
            };
        }

        private static string GetLegacyDefaultTitle(BehaviorTreeNodeKind nodeKind)
        {
            return nodeKind switch
            {
                BehaviorTreeNodeKind.Root => "Root",
                BehaviorTreeNodeKind.Sequence => "Sequence",
                BehaviorTreeNodeKind.Selector => "Selector",
                BehaviorTreeNodeKind.Parallel => "Parallel",
                BehaviorTreeNodeKind.Inverter => "Inverter",
                BehaviorTreeNodeKind.Succeeder => "Succeeder",
                BehaviorTreeNodeKind.Failer => "Failer",
                BehaviorTreeNodeKind.Repeater => "Repeater",
                BehaviorTreeNodeKind.BlackboardCondition => "Blackboard Condition",
                BehaviorTreeNodeKind.Service => "Service",
                BehaviorTreeNodeKind.Action => "Action",
                BehaviorTreeNodeKind.Condition => "Condition",
                BehaviorTreeNodeKind.Wait => "Wait",
                BehaviorTreeNodeKind.SubTree => "SubTree",
                _ => nodeKind.ToString(),
            };
        }

        private static string GetArgumentDisplayValue(BehaviorTreeArgumentDefinition argument, BehaviorTreeNodeParameterDefinition parameter)
        {
            if (argument?.Value == null)
            {
                return string.Empty;
            }

            if (parameter.EditorHint == BehaviorTreeNodeParameterEditorHint.CompareOperator)
            {
                return ((BehaviorTreeCompareOperator)BehaviorTreeValueUtility.GetInt(argument.Value, (int)BehaviorTreeCompareOperator.Equal)).ToString();
            }

            return BehaviorTreeValueUtility.ToDisplayString(argument.Value);
        }

        private static void SyncArguments(List<BehaviorTreeArgumentDefinition> arguments, IReadOnlyList<BehaviorTreeNodeParameterDefinition> parameters)
        {
            parameters ??= Array.Empty<BehaviorTreeNodeParameterDefinition>();

            Dictionary<string, BehaviorTreeArgumentDefinition> argumentMap = new(StringComparer.OrdinalIgnoreCase);
            foreach (BehaviorTreeArgumentDefinition argument in arguments)
            {
                if (argument == null || string.IsNullOrWhiteSpace(argument.Name) || argumentMap.ContainsKey(argument.Name))
                {
                    continue;
                }

                argumentMap.Add(argument.Name, argument);
            }

            arguments.Clear();
            foreach (BehaviorTreeNodeParameterDefinition parameter in parameters)
            {
                if (parameter == null || string.IsNullOrWhiteSpace(parameter.Name))
                {
                    continue;
                }

                BehaviorTreeArgumentDefinition argument = argumentMap.TryGetValue(parameter.Name, out BehaviorTreeArgumentDefinition currentArgument)
                        ? currentArgument.Clone()
                        : new BehaviorTreeArgumentDefinition
                        {
                            Name = parameter.Name,
                            Value = parameter.DefaultValue?.Clone() ?? new BehaviorTreeSerializedValue(),
                        };

                argument.Name = parameter.Name;
                argument.Value ??= parameter.DefaultValue?.Clone() ?? new BehaviorTreeSerializedValue();
                if (argument.Value.ValueType == BehaviorTreeValueType.None && parameter.DefaultValue != null)
                {
                    argument.Value = parameter.DefaultValue.Clone();
                }

                if (parameter.ValueType != BehaviorTreeValueType.None)
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

            nodeDescriptors = new Dictionary<string, ABehaviorTreeNodeDescriptor>(StringComparer.OrdinalIgnoreCase);
            handlerNodeDescriptors = new Dictionary<string, ABehaviorTreeNodeDescriptor>(StringComparer.OrdinalIgnoreCase);
            orderedNodeDescriptors = new List<ABehaviorTreeNodeDescriptor>();

            foreach (Type type in TypeCache.GetTypesWithAttribute<BehaviorTreeNodeDescriptorAttribute>())
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                if (Activator.CreateInstance(type) is not ABehaviorTreeNodeDescriptor descriptor)
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

        private static bool TryGetDescriptorByTypeId(string nodeTypeId, out ABehaviorTreeNodeDescriptor descriptor)
        {
            EnsureDescriptorCaches();
            descriptor = null;
            return !string.IsNullOrWhiteSpace(nodeTypeId) && nodeDescriptors.TryGetValue(nodeTypeId, out descriptor);
        }

        private static bool TryGetDescriptorByHandler(BehaviorTreeNodeKind nodeKind, string handlerName, out ABehaviorTreeNodeDescriptor descriptor)
        {
            EnsureDescriptorCaches();
            descriptor = null;
            return !string.IsNullOrWhiteSpace(handlerName) && handlerNodeDescriptors.TryGetValue(BuildHandlerKey(nodeKind, handlerName), out descriptor);
        }

        private static string BuildHandlerKey(BehaviorTreeNodeKind nodeKind, string handlerName)
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
            if (Attribute.GetCustomAttribute(type, attributeType) is BehaviorTreeHandlerAttribute handlerAttribute && !string.IsNullOrWhiteSpace(handlerAttribute.Name))
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

        private static bool TryGetRuntimeNodeScript(BehaviorTreeEditorNodeData node, out UnityEngine.Object scriptAsset)
        {
            scriptAsset = null;
            if (node == null)
            {
                return false;
            }

            string assetPath = node.NodeKind switch
            {
                BehaviorTreeNodeKind.Root => "Assets/Scripts/Model/Share/Module/BehaviorTree/BehaviorTreeRootNode.cs",
                BehaviorTreeNodeKind.Sequence => "Assets/Scripts/Model/Share/Module/BehaviorTree/BehaviorTreeSequenceNode.cs",
                BehaviorTreeNodeKind.Selector => "Assets/Scripts/Model/Share/Module/BehaviorTree/BehaviorTreeSelectorNode.cs",
                BehaviorTreeNodeKind.Parallel => "Assets/Scripts/Model/Share/Module/BehaviorTree/BehaviorTreeParallelNode.cs",
                BehaviorTreeNodeKind.Inverter => "Assets/Scripts/Model/Share/Module/BehaviorTree/BehaviorTreeInverterNode.cs",
                BehaviorTreeNodeKind.Succeeder => "Assets/Scripts/Model/Share/Module/BehaviorTree/BehaviorTreeSucceederNode.cs",
                BehaviorTreeNodeKind.Failer => "Assets/Scripts/Model/Share/Module/BehaviorTree/BehaviorTreeFailerNode.cs",
                BehaviorTreeNodeKind.Repeater => "Assets/Scripts/Model/Share/Module/BehaviorTree/BehaviorTreeRepeaterNode.cs",
                BehaviorTreeNodeKind.BlackboardCondition => "Assets/Scripts/Model/Share/Module/BehaviorTree/BehaviorTreeBlackboardConditionNode.cs",
                BehaviorTreeNodeKind.Wait => "Assets/Scripts/Model/Share/Module/BehaviorTree/BehaviorTreeWaitNode.cs",
                BehaviorTreeNodeKind.SubTree => "Assets/Scripts/Model/Share/Module/BehaviorTree/BehaviorTreeSubTreeNode.cs",
                BehaviorTreeNodeKind.Service => "Assets/Scripts/Model/Share/Module/BehaviorTree/BehaviorTreeServiceNode.cs",
                BehaviorTreeNodeKind.Action => "Assets/Scripts/Model/Share/Module/BehaviorTree/BehaviorTreeActionNode.cs",
                BehaviorTreeNodeKind.Condition => "Assets/Scripts/Model/Share/Module/BehaviorTree/BehaviorTreeConditionNode.cs",
                _ => string.Empty,
            };

            if (string.IsNullOrWhiteSpace(assetPath))
            {
                return false;
            }

            scriptAsset = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
            return scriptAsset != null;
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

                if (Attribute.GetCustomAttribute(type, typeof(T)) is BehaviorTreeHandlerAttribute handlerAttribute && !string.IsNullOrWhiteSpace(handlerAttribute.Name))
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
