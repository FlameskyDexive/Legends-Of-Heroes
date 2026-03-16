using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace ET
{
    public sealed class BTSerializationBenchmarkTests
    {
        [Test]
        public void BehaviorTree_NinoSerializeDeserialize_Benchmark()
        {
            const int treeCount = 24;
            const int iterations = 80;
            const int warmupIterations = 5;

            BTAsset asset = CreateBenchmarkAsset(treeCount);

            for (int i = 0; i < warmupIterations; ++i)
            {
                byte[] warmupBytes = BTExporter.BuildBytes(asset);
                object warmupPackage = BTEditorRuntimeBridge.DeserializePackage(warmupBytes);
                Assert.That(GetTreeCount(warmupPackage), Is.GreaterThan(0));
            }

            byte[] serializedBytes = null;
            Stopwatch serializeWatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; ++i)
            {
                serializedBytes = BTExporter.BuildBytes(asset);
            }

            serializeWatch.Stop();

            object roundTripPackage = null;
            Stopwatch deserializeWatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; ++i)
            {
                roundTripPackage = BTEditorRuntimeBridge.DeserializePackage(serializedBytes);
            }

            deserializeWatch.Stop();

            int treeTotal = GetTreeCount(roundTripPackage);
            int nodeTotal = CountNodes(roundTripPackage);
            TestContext.WriteLine($"BehaviorTree serialize benchmark: trees={treeTotal}, nodes={nodeTotal}, bytes={serializedBytes.Length}, iterations={iterations}, totalMs={serializeWatch.Elapsed.TotalMilliseconds:F3}, avgMs={serializeWatch.Elapsed.TotalMilliseconds / iterations:F4}");
            TestContext.WriteLine($"BehaviorTree deserialize benchmark: trees={treeTotal}, nodes={nodeTotal}, bytes={serializedBytes.Length}, iterations={iterations}, totalMs={deserializeWatch.Elapsed.TotalMilliseconds:F3}, avgMs={deserializeWatch.Elapsed.TotalMilliseconds / iterations:F4}");

            ScriptableObject.DestroyImmediate(asset);
        }

        private static BTAsset CreateBenchmarkAsset(int treeCount)
        {
            BTAsset asset = ScriptableObject.CreateInstance<BTAsset>();
            asset.name = "BehaviorTreeSerializationBenchmark";
            asset.TreeId = "benchmark.behavior_tree.package";
            asset.TreeName = "BehaviorTreeSerializationBenchmark";
            asset.Description = "BehaviorTree benchmark asset";
            asset.EnsureInitialized();
            asset.Nodes.Clear();
            asset.BlackboardEntries.Clear();

            BTEditorNodeData root = new() { NodeId = "root", NodeKind = BTNodeKind.Root, Title = "Root" };
            BTEditorNodeData current = root;
            asset.Nodes.Add(root);

            for (int index = 0; index < treeCount; ++index)
            {
                BTEditorNodeData sequence = new() { NodeId = $"seq_{index}", NodeKind = BTNodeKind.Sequence, Title = $"Sequence {index}" };
                current.ChildIds.Add(sequence.NodeId);
                asset.Nodes.Add(sequence);

                BTEditorNodeData log = new()
                {
                    NodeId = $"log_{index}",
                    NodeKind = BTNodeKind.Action,
                    NodeTypeId = BTBuiltinNodeTypes.Log,
                    HandlerName = "Log",
                    Title = $"Log {index}",
                    Arguments = { CreateStringArgument("message", $"benchmark log {index}") },
                };
                BTEditorNodeData wait = new() { NodeId = $"wait_{index}", NodeKind = BTNodeKind.Wait, Title = $"Wait {index}", WaitMilliseconds = 50 + index };
                sequence.ChildIds.Add(log.NodeId);
                sequence.ChildIds.Add(wait.NodeId);
                asset.Nodes.Add(log);
                asset.Nodes.Add(wait);
                current = sequence;
            }

            asset.RootNodeId = root.NodeId;
            return asset;
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

        private static int GetTreeCount(object package)
        {
            return BTEditorRuntimeBridge.GetList(package, "Trees").Count;
        }

        private static int CountNodes(object package)
        {
            int count = 0;
            foreach (object tree in BTEditorRuntimeBridge.GetList(package, "Trees"))
            {
                count += BTEditorRuntimeBridge.GetList(tree, "Nodes").Count;
            }

            return count;
        }
    }
}
