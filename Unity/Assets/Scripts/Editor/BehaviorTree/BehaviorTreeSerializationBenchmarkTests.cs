using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ET.Client;
using NUnit.Framework;

namespace ET
{
    public sealed class BehaviorTreeSerializationBenchmarkTests
    {
        [Test]
        public void BehaviorTree_NinoSerializeDeserialize_Benchmark()
        {
            const int treePairCount = 24;
            const int iterations = 80;
            const int warmupIterations = 5;

            BehaviorTreePackage package = CreateBenchmarkPackage(treePairCount);

            for (int i = 0; i < warmupIterations; ++i)
            {
                byte[] warmupBytes = BehaviorTreeSerializer.Serialize(package);
                BehaviorTreePackage warmupPackage = BehaviorTreeSerializer.Deserialize(warmupBytes);
                AssertRoundTrip(package, warmupPackage);
            }

            byte[] serializedBytes = null;
            Stopwatch serializeWatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; ++i)
            {
                serializedBytes = BehaviorTreeSerializer.Serialize(package);
            }

            serializeWatch.Stop();

            BehaviorTreePackage roundTripPackage = null;
            Stopwatch deserializeWatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; ++i)
            {
                roundTripPackage = BehaviorTreeSerializer.Deserialize(serializedBytes);
            }

            deserializeWatch.Stop();

            AssertRoundTrip(package, roundTripPackage);

            double serializeAverageMs = serializeWatch.Elapsed.TotalMilliseconds / iterations;
            double deserializeAverageMs = deserializeWatch.Elapsed.TotalMilliseconds / iterations;
            TestContext.WriteLine($"BehaviorTree serialize benchmark: trees={package.Trees.Count}, nodes={CountNodes(package)}, bytes={serializedBytes.Length}, iterations={iterations}, totalMs={serializeWatch.Elapsed.TotalMilliseconds:F3}, avgMs={serializeAverageMs:F4}");
            TestContext.WriteLine($"BehaviorTree deserialize benchmark: trees={roundTripPackage.Trees.Count}, nodes={CountNodes(roundTripPackage)}, bytes={serializedBytes.Length}, iterations={iterations}, totalMs={deserializeWatch.Elapsed.TotalMilliseconds:F3}, avgMs={deserializeAverageMs:F4}");
        }

        private static BehaviorTreePackage CreateBenchmarkPackage(int treePairCount)
        {
            BehaviorTreePackage package = new()
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

        private static BehaviorTreeDefinition CreateMainTree(int index, string subTreeId, string subTreeName)
        {
            string treeId = $"benchmark.main.{index}";
            BehaviorTreeDefinition tree = new()
            {
                TreeId = treeId,
                TreeName = $"BenchmarkMain{index}",
                Description = $"Main benchmark tree {index}",
                RootNodeId = $"root_{index}",
            };

            tree.BlackboardEntries.Add(new BehaviorTreeBlackboardEntryDefinition
            {
                Key = $"HasTarget_{index}",
                ValueType = BehaviorTreeValueType.Boolean,
                DefaultValue = new BehaviorTreeSerializedValue
                {
                    ValueType = BehaviorTreeValueType.Boolean,
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

            tree.Nodes.Add(new BTDemoPatrolNodeData
            {
                NodeId = $"patrol_{index}",
                Title = "Patrol",
                PatrolPoints =
                {
                    new BehaviorTreePatrolPointDefinition { X = 0 + index, Y = 0, Z = 0 },
                    new BehaviorTreePatrolPointDefinition { X = 10 + index, Y = 0, Z = 0 },
                    new BehaviorTreePatrolPointDefinition { X = 10 + index, Y = 0, Z = 10 },
                    new BehaviorTreePatrolPointDefinition { X = 0 + index, Y = 0, Z = 10 },
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
                CompareOperator = BehaviorTreeCompareOperator.IsTrue,
                CompareValue = new BehaviorTreeSerializedValue
                {
                    ValueType = BehaviorTreeValueType.Boolean,
                    BoolValue = true,
                },
                AbortMode = BehaviorTreeAbortMode.Both,
            });

            tree.Nodes.Add(new BTParallelNodeData
            {
                NodeId = $"parallel_{index}",
                Title = "Parallel",
                SuccessPolicy = BehaviorTreeParallelPolicy.RequireAll,
                FailurePolicy = BehaviorTreeParallelPolicy.RequireOne,
                ChildIds = { $"parallel_log_{index}", $"parallel_wait_{index}" },
            });

            tree.Nodes.Add(new BTActionNodeData
            {
                NodeId = $"parallel_log_{index}",
                Title = "Parallel Log",
                TypeId = BehaviorTreeBuiltinNodeTypes.Log,
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
                TypeId = BehaviorTreeBuiltinNodeTypes.Log,
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
                TypeId = BehaviorTreeBuiltinNodeTypes.SetBlackboard,
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
                TypeId = BehaviorTreeBuiltinNodeTypes.Log,
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
                TypeId = BehaviorTreeBuiltinNodeTypes.BlackboardCompare,
                ConditionHandlerName = "BlackboardCompare",
                Arguments =
                {
                    CreateStringArgument("key", $"HasTarget_{index}"),
                    CreateIntArgument("operator", (int)BehaviorTreeCompareOperator.Equal),
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

        private static BehaviorTreeDefinition CreateSubTree(int index, string treeId, string treeName)
        {
            BehaviorTreeDefinition tree = new()
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
                TypeId = BehaviorTreeBuiltinNodeTypes.Log,
                ActionHandlerName = "Log",
                Arguments =
                {
                    CreateStringArgument("message", $"sub tree log {index}"),
                },
            });

            return tree;
        }

        private static BehaviorTreeArgumentDefinition CreateStringArgument(string name, string value)
        {
            return new BehaviorTreeArgumentDefinition
            {
                Name = name,
                Value = new BehaviorTreeSerializedValue
                {
                    ValueType = BehaviorTreeValueType.String,
                    StringValue = value,
                },
            };
        }

        private static BehaviorTreeArgumentDefinition CreateBoolArgument(string name, bool value)
        {
            return new BehaviorTreeArgumentDefinition
            {
                Name = name,
                Value = new BehaviorTreeSerializedValue
                {
                    ValueType = BehaviorTreeValueType.Boolean,
                    BoolValue = value,
                },
            };
        }

        private static BehaviorTreeArgumentDefinition CreateIntArgument(string name, int value)
        {
            return new BehaviorTreeArgumentDefinition
            {
                Name = name,
                Value = new BehaviorTreeSerializedValue
                {
                    ValueType = BehaviorTreeValueType.Integer,
                    IntValue = value,
                },
            };
        }

        private static void AssertRoundTrip(BehaviorTreePackage expected, BehaviorTreePackage actual)
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
            Assert.That(actual.Trees.SelectMany(tree => tree.Nodes).OfType<BTDemoPatrolNodeData>().Count(),
                Is.EqualTo(expected.Trees.SelectMany(tree => tree.Nodes).OfType<BTDemoPatrolNodeData>().Count()));

            BTDemoPatrolNodeData patrolNode = actual.Trees
                .SelectMany(tree => tree.Nodes)
                .OfType<BTDemoPatrolNodeData>()
                .FirstOrDefault();
            Assert.That(patrolNode, Is.Not.Null);
            Assert.That(patrolNode.PatrolPoints.Count, Is.EqualTo(4));
            Assert.That(patrolNode.PatrolPoints[1].X, Is.GreaterThan(patrolNode.PatrolPoints[0].X));
        }

        private static int CountNodes(BehaviorTreePackage package)
        {
            return package.Trees.Sum(tree => tree.Nodes.Count);
        }
    }
}
