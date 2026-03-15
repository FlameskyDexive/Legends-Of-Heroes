using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ET.Client;
using NUnit.Framework;

namespace ET
{
    public sealed class BTSerializationBenchmarkTests
    {
        [Test]
        public void BehaviorTree_NinoSerializeDeserialize_Benchmark()
        {
            const int treePairCount = 24;
            const int iterations = 80;
            const int warmupIterations = 5;

            BTPackage package = CreateBenchmarkPackage(treePairCount);

            for (int i = 0; i < warmupIterations; ++i)
            {
                byte[] warmupBytes = BTSerializer.Serialize(package);
                BTPackage warmupPackage = BTSerializer.Deserialize(warmupBytes);
                AssertRoundTrip(package, warmupPackage);
            }

            byte[] serializedBytes = null;
            Stopwatch serializeWatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; ++i)
            {
                serializedBytes = BTSerializer.Serialize(package);
            }

            serializeWatch.Stop();

            BTPackage roundTripPackage = null;
            Stopwatch deserializeWatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; ++i)
            {
                roundTripPackage = BTSerializer.Deserialize(serializedBytes);
            }

            deserializeWatch.Stop();

            AssertRoundTrip(package, roundTripPackage);

            double serializeAverageMs = serializeWatch.Elapsed.TotalMilliseconds / iterations;
            double deserializeAverageMs = deserializeWatch.Elapsed.TotalMilliseconds / iterations;
            TestContext.WriteLine($"BehaviorTree serialize benchmark: trees={package.Trees.Count}, nodes={CountNodes(package)}, bytes={serializedBytes.Length}, iterations={iterations}, totalMs={serializeWatch.Elapsed.TotalMilliseconds:F3}, avgMs={serializeAverageMs:F4}");
            TestContext.WriteLine($"BehaviorTree deserialize benchmark: trees={roundTripPackage.Trees.Count}, nodes={CountNodes(roundTripPackage)}, bytes={serializedBytes.Length}, iterations={iterations}, totalMs={deserializeWatch.Elapsed.TotalMilliseconds:F3}, avgMs={deserializeAverageMs:F4}");
        }

        private static BTPackage CreateBenchmarkPackage(int treePairCount)
        {
            BTPackage package = new()
            {
                PackageId = "benchmark.behavior_tree.package",
                PackageName = "BehaviorTreeSerializationBenchmark",
                EntryTreeId = "benchmark.main.0",
                EntryTreeName = "BenchmarkMain0",
            };

            for (int i = 0; i < treePairCount; ++i)
            {
                string subTreeId = $"benchmark.sub.{i}";
                string subTreeName = $"BenchmarkSub{i}";
                package.Trees.Add(CreateSubTree(i, subTreeId, subTreeName));
                package.Trees.Add(CreateMainTree(i, subTreeId, subTreeName));
            }

            return package;
        }

        private static BTDefinition CreateMainTree(int index, string subTreeId, string subTreeName)
        {
            string treeId = $"benchmark.main.{index}";
            BTDefinition tree = new()
            {
                TreeId = treeId,
                TreeName = $"BenchmarkMain{index}",
                Description = $"Main benchmark tree {index}",
                RootNodeId = $"root_{index}",
            };

            tree.BlackboardEntries.Add(new BTBlackboardEntryData
            {
                Key = $"HasTarget_{index}",
                ValueType = BTValueType.Boolean,
                DefaultValue = new BTSerializedValue
                {
                    ValueType = BTValueType.Boolean,
                    BoolValue = true,
                },
                Description = "Benchmark blackboard bool value",
            });

            tree.Nodes.Add(BTEditorRuntimeNodeFactory.CreateRootNode($"root_{index}", "Root", childIds: new[] { $"seq_{index}" }));
            tree.Nodes.Add(BTEditorRuntimeNodeFactory.CreateSequenceNode($"seq_{index}", "Main Sequence", childIds: new[]
            {
                $"patrol_{index}",
                $"wait_{index}",
                $"bb_{index}",
                $"parallel_{index}",
                $"repeat_{index}",
                $"service_{index}",
                $"condition_{index}",
                $"subtree_{index}",
            }));
            tree.Nodes.Add(BTEditorRuntimeNodeFactory.CreatePatrolNode($"patrol_{index}", "Patrol", string.Empty, new[]
            {
                new BTPatrolPointData { X = 0 + index, Y = 0, Z = 0 },
                new BTPatrolPointData { X = 10 + index, Y = 0, Z = 0 },
                new BTPatrolPointData { X = 10 + index, Y = 0, Z = 10 },
                new BTPatrolPointData { X = 0 + index, Y = 0, Z = 10 },
            }));
            tree.Nodes.Add(BTEditorRuntimeNodeFactory.CreateWaitNode($"wait_{index}", "Wait", string.Empty, 250 + index));
            tree.Nodes.Add(BTEditorRuntimeNodeFactory.CreateBlackboardConditionNode($"bb_{index}", "Blackboard Condition", string.Empty,
                $"HasTarget_{index}",
                BTCompareOperator.IsTrue,
                new BTSerializedValue
                {
                    ValueType = BTValueType.Boolean,
                    BoolValue = true,
                },
                BTAbortMode.Both));
            tree.Nodes.Add(BTEditorRuntimeNodeFactory.CreateParallelNode($"parallel_{index}", "Parallel", string.Empty,
                BTParallelPolicy.RequireAll, BTParallelPolicy.RequireOne, new[] { $"parallel_log_{index}", $"parallel_wait_{index}" }));
            tree.Nodes.Add(BTEditorRuntimeNodeFactory.CreateActionNode($"parallel_log_{index}", "Parallel Log", string.Empty,
                BTBuiltinNodeTypes.Log, "Log", new[] { CreateStringArgument("message", $"parallel log {index}") }));
            tree.Nodes.Add(BTEditorRuntimeNodeFactory.CreateWaitNode($"parallel_wait_{index}", "Parallel Wait", string.Empty, 50 + index));
            tree.Nodes.Add(BTEditorRuntimeNodeFactory.CreateRepeaterNode($"repeat_{index}", "Repeat", string.Empty, 2 + (index % 3), new[] { $"repeat_log_{index}" }));
            tree.Nodes.Add(BTEditorRuntimeNodeFactory.CreateActionNode($"repeat_log_{index}", "Repeat Log", string.Empty,
                BTBuiltinNodeTypes.Log, "Log", new[] { CreateStringArgument("message", $"repeat log {index}") }));
            tree.Nodes.Add(BTEditorRuntimeNodeFactory.CreateServiceNode($"service_{index}", "Service", string.Empty,
                BTBuiltinNodeTypes.SetBlackboard, "SetBlackboard", 100 + index,
                new[]
                {
                    CreateStringArgument("key", $"RuntimeKey_{index}"),
                    CreateBoolArgument("remove", false),
                    CreateStringArgument("value", $"RuntimeValue_{index}"),
                },
                new[] { $"service_leaf_{index}" }));
            tree.Nodes.Add(BTEditorRuntimeNodeFactory.CreateActionNode($"service_leaf_{index}", "Service Leaf", string.Empty,
                BTBuiltinNodeTypes.Log, "Log", new[] { CreateStringArgument("message", $"service leaf {index}") }));
            tree.Nodes.Add(BTEditorRuntimeNodeFactory.CreateConditionNode($"condition_{index}", "Condition", string.Empty,
                BTBuiltinNodeTypes.BlackboardCompare, "BlackboardCompare",
                new[]
                {
                    CreateStringArgument("key", $"HasTarget_{index}"),
                    CreateIntArgument("operator", (int)BTCompareOperator.Equal),
                    CreateBoolArgument("value", true),
                }));
            tree.Nodes.Add(BTEditorRuntimeNodeFactory.CreateSubTreeNode($"subtree_{index}", "SubTree", string.Empty, subTreeId, subTreeName));

            return tree;
        }

        private static BTDefinition CreateSubTree(int index, string treeId, string treeName)
        {
            BTDefinition tree = new()
            {
                TreeId = treeId,
                TreeName = treeName,
                Description = $"Sub benchmark tree {index}",
                RootNodeId = $"sub_root_{index}",
            };

            tree.Nodes.Add(BTEditorRuntimeNodeFactory.CreateRootNode($"sub_root_{index}", "Sub Root", childIds: new[] { $"sub_log_{index}" }));
            tree.Nodes.Add(BTEditorRuntimeNodeFactory.CreateActionNode($"sub_log_{index}", "Sub Log", string.Empty,
                BTBuiltinNodeTypes.Log, "Log", new[] { CreateStringArgument("message", $"sub tree log {index}") }));

            return tree;
        }

        private static BTArgumentData CreateStringArgument(string name, string value)
        {
            return new BTArgumentData
            {
                Name = name,
                Value = new BTSerializedValue
                {
                    ValueType = BTValueType.String,
                    StringValue = value,
                },
            };
        }

        private static BTArgumentData CreateBoolArgument(string name, bool value)
        {
            return new BTArgumentData
            {
                Name = name,
                Value = new BTSerializedValue
                {
                    ValueType = BTValueType.Boolean,
                    BoolValue = value,
                },
            };
        }

        private static BTArgumentData CreateIntArgument(string name, int value)
        {
            return new BTArgumentData
            {
                Name = name,
                Value = new BTSerializedValue
                {
                    ValueType = BTValueType.Integer,
                    IntValue = value,
                },
            };
        }

        private static void AssertRoundTrip(BTPackage expected, BTPackage actual)
        {
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.PackageId, Is.EqualTo(expected.PackageId));
            Assert.That(actual.PackageName, Is.EqualTo(expected.PackageName));
            Assert.That(actual.EntryTreeId, Is.EqualTo(expected.EntryTreeId));
            Assert.That(actual.Trees.Count, Is.EqualTo(expected.Trees.Count));
            Assert.That(CountNodes(actual), Is.EqualTo(CountNodes(expected)));

            Assert.That(actual.Trees.SelectMany(tree => tree.Nodes).Count(node => BTEditorRuntimeNodeFactory.IsRuntimeNodeType(node, "BTWaitNodeData")),
                Is.EqualTo(expected.Trees.SelectMany(tree => tree.Nodes).Count(node => BTEditorRuntimeNodeFactory.IsRuntimeNodeType(node, "BTWaitNodeData"))));
            Assert.That(actual.Trees.SelectMany(tree => tree.Nodes).Count(node => BTEditorRuntimeNodeFactory.IsRuntimeNodeType(node, "BTParallelNodeData")),
                Is.EqualTo(expected.Trees.SelectMany(tree => tree.Nodes).Count(node => BTEditorRuntimeNodeFactory.IsRuntimeNodeType(node, "BTParallelNodeData"))));
            Assert.That(actual.Trees.SelectMany(tree => tree.Nodes).Count(node => BTEditorRuntimeNodeFactory.IsRuntimeNodeType(node, "BTPatrolNodeData")),
                Is.EqualTo(expected.Trees.SelectMany(tree => tree.Nodes).Count(node => BTEditorRuntimeNodeFactory.IsRuntimeNodeType(node, "BTPatrolNodeData"))));

            BTNodeData patrolNode = actual.Trees
                .SelectMany(tree => tree.Nodes)
                .FirstOrDefault(node => BTEditorRuntimeNodeFactory.IsRuntimeNodeType(node, "BTPatrolNodeData"));
            Assert.That(patrolNode, Is.Not.Null);
            List<BTPatrolPointData> patrolPoints = BTEditorRuntimeNodeFactory.GetPatrolPoints(patrolNode);
            Assert.That(patrolPoints.Count, Is.EqualTo(4));
            Assert.That(patrolPoints[1].X, Is.GreaterThan(patrolPoints[0].X));
        }

        private static int CountNodes(BTPackage package)
        {
            return package.Trees.Sum(tree => tree.Nodes.Count);
        }
    }
}
