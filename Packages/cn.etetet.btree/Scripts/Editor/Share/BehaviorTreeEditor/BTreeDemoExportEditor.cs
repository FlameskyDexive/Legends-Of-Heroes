using System.IO;
using UnityEditor;
using UnityEngine;

namespace ET
{
    public static class BTreeDemoExportEditor
    {
        [MenuItem("ET/AI/Export Demo AITest.bytes", false, 1008)]
        public static void ExportDemoAITest()
        {
            byte[] bytes = CreateAITestBytes();
            string clientFilePath = Path.Combine(BTreeBytesLoader.ClientBehaviorTreeBytesDir, "AITest.bytes");
            string clientDirectory = Path.GetDirectoryName(clientFilePath);
            if (!Directory.Exists(clientDirectory))
            {
                Directory.CreateDirectory(clientDirectory);
            }

            string serverFilePath = Path.Combine(BTreeBytesLoader.ServerBehaviorTreeBytesDir, "AITest.bytes");
            string serverDirectory = Path.GetDirectoryName(serverFilePath);
            if (!Directory.Exists(serverDirectory))
            {
                Directory.CreateDirectory(serverDirectory);
            }

            File.WriteAllBytes(clientFilePath, bytes);
            File.WriteAllBytes(serverFilePath, bytes);
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("BehaviorTree", $"Exported demo files:\n{clientFilePath}\n{serverFilePath}", "OK");
        }

        private static byte[] CreateAITestBytes()
        {
            BTreeAsset asset = ScriptableObject.CreateInstance<BTreeAsset>();
            asset.name = "AITest";
            asset.TreeId = "demo.shared.ai_test";
            asset.TreeName = "AITest";
            asset.Description = "Shared client/server demo behavior tree.";
            asset.EnsureInitialized();
            asset.Nodes.Clear();
            asset.BlackboardEntries.Clear();

            BTreeEditorNodeData root = new()
            {
                NodeId = "root",
                NodeKind = BTreeNodeKind.Root,
                Title = "Root",
                ChildIds = { "repeat" },
            };
            BTreeEditorNodeData repeat = new()
            {
                NodeId = "repeat",
                NodeKind = BTreeNodeKind.Repeater,
                Title = "Repeat Tick",
                ChildIds = { "sequence" },
            };
            BTreeEditorNodeData sequence = new()
            {
                NodeId = "sequence",
                NodeKind = BTreeNodeKind.Sequence,
                Title = "Tick Sequence",
                ChildIds = { "log", "wait" },
            };
            BTreeEditorNodeData log = new()
            {
                NodeId = "log",
                NodeKind = BTreeNodeKind.Action,
                NodeTypeId = BTreeBuiltinNodeTypes.Log,
                HandlerName = "Log",
                Title = "Log Tick",
                Arguments =
                {
                    new BTreeArgumentData
                    {
                        Name = "message",
                        Value = new BTreeSerializedValue
                        {
                            ValueType = BTreeValueType.String,
                            StringValue = "AITest tick",
                        },
                    },
                },
            };
            BTreeEditorNodeData wait = new()
            {
                NodeId = "wait",
                NodeKind = BTreeNodeKind.Wait,
                Title = "Wait",
                WaitMilliseconds = 1000,
            };

            asset.Nodes.Add(root);
            asset.Nodes.Add(repeat);
            asset.Nodes.Add(sequence);
            asset.Nodes.Add(log);
            asset.Nodes.Add(wait);
            asset.RootNodeId = root.NodeId;

            byte[] bytes = BTreeExporter.BuildBytes(asset);
            ScriptableObject.DestroyImmediate(asset);
            return bytes;
        }
    }
}
