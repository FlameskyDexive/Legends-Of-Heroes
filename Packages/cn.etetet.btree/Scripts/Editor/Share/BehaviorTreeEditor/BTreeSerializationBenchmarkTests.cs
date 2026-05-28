using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace ET
{
    public sealed class BTreeSerializationBenchmarkTests
    {
        [Test]
        public void BehaviorTree_NinoSerializeDeserialize_Benchmark()
        {
            const int treeCount = 24;
            const int iterations = 80;
            const int warmupIterations = 5;

            BTreeAsset asset = CreateBenchmarkAsset(treeCount);

            for (int i = 0; i < warmupIterations; ++i)
            {
                byte[] warmupBytes = BTreeExporter.BuildBytes(asset);
                object warmupPackage = BTreeEditorRuntimeBridge.DeserializePackage(warmupBytes);
                Assert.That(GetTreeCount(warmupPackage), Is.GreaterThan(0));
            }

            byte[] serializedBytes = null;
            Stopwatch serializeWatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; ++i)
            {
                serializedBytes = BTreeExporter.BuildBytes(asset);
            }

            serializeWatch.Stop();

            object roundTripPackage = null;
            Stopwatch deserializeWatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; ++i)
            {
                roundTripPackage = BTreeEditorRuntimeBridge.DeserializePackage(serializedBytes);
            }

            deserializeWatch.Stop();

            int treeTotal = GetTreeCount(roundTripPackage);
            int nodeTotal = CountNodes(roundTripPackage);
            TestContext.WriteLine($"BehaviorTree serialize benchmark: trees={treeTotal}, nodes={nodeTotal}, bytes={serializedBytes.Length}, iterations={iterations}, totalMs={serializeWatch.Elapsed.TotalMilliseconds:F3}, avgMs={serializeWatch.Elapsed.TotalMilliseconds / iterations:F4}");
            TestContext.WriteLine($"BehaviorTree deserialize benchmark: trees={treeTotal}, nodes={nodeTotal}, bytes={serializedBytes.Length}, iterations={iterations}, totalMs={deserializeWatch.Elapsed.TotalMilliseconds:F3}, avgMs={deserializeWatch.Elapsed.TotalMilliseconds / iterations:F4}");

            ScriptableObject.DestroyImmediate(asset);
        }

        private static BTreeAsset CreateBenchmarkAsset(int treeCount)
        {
            BTreeAsset asset = ScriptableObject.CreateInstance<BTreeAsset>();
            asset.name = "BehaviorTreeSerializationBenchmark";
            asset.TreeId = "benchmark.behavior_tree.package";
            asset.TreeName = "BehaviorTreeSerializationBenchmark";
            asset.Description = "BehaviorTree benchmark asset";
            asset.EnsureInitialized();
            asset.Nodes.Clear();
            asset.BlackboardEntries.Clear();

            BTreeEditorNodeData root = new() { NodeId = "root", NodeKind = BTreeNodeKind.Root, Title = "Root" };
            BTreeEditorNodeData current = root;
            asset.Nodes.Add(root);

            for (int index = 0; index < treeCount; ++index)
            {
                BTreeEditorNodeData sequence = new() { NodeId = $"seq_{index}", NodeKind = BTreeNodeKind.Sequence, Title = $"Sequence {index}" };
                current.ChildIds.Add(sequence.NodeId);
                asset.Nodes.Add(sequence);

                BTreeEditorNodeData log = new()
                {
                    NodeId = $"log_{index}",
                    NodeKind = BTreeNodeKind.Action,
                    NodeTypeId = BTreeBuiltinNodeTypes.Log,
                    HandlerName = "Log",
                    Title = $"Log {index}",
                    Arguments = { CreateStringArgument("message", $"benchmark log {index}") },
                };
                BTreeEditorNodeData wait = new() { NodeId = $"wait_{index}", NodeKind = BTreeNodeKind.Wait, Title = $"Wait {index}", WaitMilliseconds = 50 + index };
                sequence.ChildIds.Add(log.NodeId);
                sequence.ChildIds.Add(wait.NodeId);
                asset.Nodes.Add(log);
                asset.Nodes.Add(wait);
                current = sequence;
            }

            asset.RootNodeId = root.NodeId;
            return asset;
        }

        private static BTreeArgumentData CreateStringArgument(string name, string value)
        {
            return new BTreeArgumentData
            {
                Name = name,
                Value = new BTreeSerializedValue
                {
                    ValueType = BTreeValueType.String,
                    StringValue = value,
                },
            };
        }

        private static int GetTreeCount(object package)
        {
            return BTreeEditorRuntimeBridge.GetList(package, "Trees").Count;
        }

        private static int CountNodes(object package)
        {
            int count = 0;
            foreach (object tree in BTreeEditorRuntimeBridge.GetList(package, "Trees"))
            {
                count += BTreeEditorRuntimeBridge.GetList(tree, "Nodes").Count;
            }

            return count;
        }
    }
}
