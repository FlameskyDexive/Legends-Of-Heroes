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

            tree.Nodes.Add(new BTRootNodeData
            {
                NodeId = $"root_{index}",
                Title = "Root",
                ChildIds = { $"seq_{index}" },
            });

            tree.Nodes.Add(new BTSequenceNodeData
            {
                NodeId = $"seq_{index}",
                Title = "Main Sequence",
                ChildIds =
                {
                    $"patrol_{index}",
                    $"wait_{index}",
                    $"bb_{index}",
                    $"parallel_{index}",
                    $"repeat_{index}",
                    $"service_{index}",
                    $"condition_{index}",
                    $"subtree_{index}",
                },
            });

            tree.Nodes.Add(new BTPatrolNodeData
            {
                NodeId = $"patrol_{index}",
                Title = "Patrol",
                PatrolPoints =
                {
                    new BTPatrolPointData { X = 0 + index, Y = 0, Z = 0 },
                    new BTPatrolPointData { X = 10 + index, Y = 0, Z = 0 },
                    new BTPatrolPointData { X = 10 + index, Y = 0, Z = 10 },
                    new BTPatrolPointData { X = 0 + index, Y = 0, Z = 10 },
                },
            });

            tree.Nodes.Add(new BTWaitNodeData
            {
                NodeId = $"wait_{index}",
                Title = "Wait",
                WaitMilliseconds = 250 + index,
            });

            tree.Nodes.Add(new BTBlackboardConditionNodeData
            {
                NodeId = $"bb_{index}",
                Title = "Blackboard Condition",
                BlackboardKey = $"HasTarget_{index}",
                CompareOperator = BTCompareOperator.IsTrue,
                CompareValue = new BTSerializedValue
                {
                    ValueType = BTValueType.Boolean,
                    BoolValue = true,
                },
                AbortMode = BTAbortMode.Both,
            });

            tree.Nodes.Add(new BTParallelNodeData
            {
                NodeId = $"parallel_{index}",
                Title = "Parallel",
                SuccessPolicy = BTParallelPolicy.RequireAll,
                FailurePolicy = BTParallelPolicy.RequireOne,
                ChildIds = { $"parallel_log_{index}", $"parallel_wait_{index}" },
            });

            tree.Nodes.Add(new BTActionNodeData
            {
                NodeId = $"parallel_log_{index}",
                Title = "Parallel Log",
                TypeId = BTBuiltinNodeTypes.Log,
                ActionHandlerName = "Log",
                Arguments =
                {
                    CreateStringArgument("message", $"parallel log {index}"),
                },
            });

            tree.Nodes.Add(new BTWaitNodeData
            {
                NodeId = $"parallel_wait_{index}",
                Title = "Parallel Wait",
                WaitMilliseconds = 50 + index,
            });

            tree.Nodes.Add(new BTRepeaterNodeData
            {
                NodeId = $"repeat_{index}",
                Title = "Repeat",
                MaxLoopCount = 2 + (index % 3),
                ChildIds = { $"repeat_log_{index}" },
            });

            tree.Nodes.Add(new BTActionNodeData
            {
                NodeId = $"repeat_log_{index}",
                Title = "Repeat Log",
                TypeId = BTBuiltinNodeTypes.Log,
                ActionHandlerName = "Log",
                Arguments =
                {
                    CreateStringArgument("message", $"repeat log {index}"),
                },
            });

            tree.Nodes.Add(new BTServiceNodeData
            {
                NodeId = $"service_{index}",
                Title = "Service",
                TypeId = BTBuiltinNodeTypes.SetBlackboard,
                ServiceHandlerName = "SetBlackboard",
                IntervalMilliseconds = 100 + index,
                Arguments =
                {
                    CreateStringArgument("key", $"RuntimeKey_{index}"),
                    CreateBoolArgument("remove", false),
                    CreateStringArgument("value", $"RuntimeValue_{index}"),
                },
                ChildIds = { $"service_leaf_{index}" },
            });

            tree.Nodes.Add(new BTActionNodeData
            {
                NodeId = $"service_leaf_{index}",
                Title = "Service Leaf",
                TypeId = BTBuiltinNodeTypes.Log,
                ActionHandlerName = "Log",
                Arguments =
                {
                    CreateStringArgument("message", $"service leaf {index}"),
                },
            });

            tree.Nodes.Add(new BTConditionNodeData
            {
                NodeId = $"condition_{index}",
                Title = "Condition",
                TypeId = BTBuiltinNodeTypes.BlackboardCompare,
                ConditionHandlerName = "BlackboardCompare",
                Arguments =
                {
                    CreateStringArgument("key", $"HasTarget_{index}"),
                    CreateIntArgument("operator", (int)BTCompareOperator.Equal),
                    CreateBoolArgument("value", true),
                },
            });

            tree.Nodes.Add(new BTSubTreeNodeData
            {
                NodeId = $"subtree_{index}",
                Title = "SubTree",
                SubTreeId = subTreeId,
                SubTreeName = subTreeName,
            });

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

            tree.Nodes.Add(new BTRootNodeData
            {
                NodeId = $"sub_root_{index}",
                Title = "Sub Root",
                ChildIds = { $"sub_log_{index}" },
            });

            tree.Nodes.Add(new BTActionNodeData
            {
                NodeId = $"sub_log_{index}",
                Title = "Sub Log",
                TypeId = BTBuiltinNodeTypes.Log,
                ActionHandlerName = "Log",
                Arguments =
                {
                    CreateStringArgument("message", $"sub tree log {index}"),
                },
            });

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

            Assert.That(actual.Trees.SelectMany(tree => tree.Nodes).OfType<BTWaitNodeData>().Count(),
                Is.EqualTo(expected.Trees.SelectMany(tree => tree.Nodes).OfType<BTWaitNodeData>().Count()));
            Assert.That(actual.Trees.SelectMany(tree => tree.Nodes).OfType<BTParallelNodeData>().Count(),
                Is.EqualTo(expected.Trees.SelectMany(tree => tree.Nodes).OfType<BTParallelNodeData>().Count()));
            Assert.That(actual.Trees.SelectMany(tree => tree.Nodes).OfType<BTPatrolNodeData>().Count(),
                Is.EqualTo(expected.Trees.SelectMany(tree => tree.Nodes).OfType<BTPatrolNodeData>().Count()));

            BTPatrolNodeData patrolNode = actual.Trees
                .SelectMany(tree => tree.Nodes)
                .OfType<BTPatrolNodeData>()
                .FirstOrDefault();
            Assert.That(patrolNode, Is.Not.Null);
            Assert.That(patrolNode.PatrolPoints.Count, Is.EqualTo(4));
            Assert.That(patrolNode.PatrolPoints[1].X, Is.GreaterThan(patrolNode.PatrolPoints[0].X));
        }

        private static int CountNodes(BTPackage package)
        {
            return package.Trees.Sum(tree => tree.Nodes.Count);
        }
    }
}
