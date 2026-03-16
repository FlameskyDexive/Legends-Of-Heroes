using System.IO;
using UnityEditor;
using UnityEngine;

namespace ET
{
    public static class BTDemoExportEditor
    {
        [MenuItem("ET/AI/Export Demo AITest.bytes", false, 1008)]
        public static void ExportDemoAITest()
        {
            byte[] bytes = CreateAITestBytes();
            string clientFilePath = Path.Combine(BTBytesLoader.ClientBehaviorTreeBytesDir, "AITest.bytes");
            string clientDirectory = Path.GetDirectoryName(clientFilePath);
            if (!Directory.Exists(clientDirectory))
            {
                Directory.CreateDirectory(clientDirectory);
            }

            string serverFilePath = Path.Combine(BTBytesLoader.ServerBehaviorTreeBytesDir, "AITest.bytes");
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
            BTAsset asset = ScriptableObject.CreateInstance<BTAsset>();
            asset.name = "AITest";
            asset.TreeId = "demo.shared.ai_test";
            asset.TreeName = "AITest";
            asset.Description = "Shared client/server demo behavior tree.";
            asset.EnsureInitialized();
            asset.Nodes.Clear();
            asset.BlackboardEntries.Clear();

            BTEditorNodeData root = new()
            {
                NodeId = "root",
                NodeKind = BTNodeKind.Root,
                Title = "Root",
                ChildIds = { "repeat" },
            };
            BTEditorNodeData repeat = new()
            {
                NodeId = "repeat",
                NodeKind = BTNodeKind.Repeater,
                Title = "Repeat Tick",
                ChildIds = { "sequence" },
            };
            BTEditorNodeData sequence = new()
            {
                NodeId = "sequence",
                NodeKind = BTNodeKind.Sequence,
                Title = "Tick Sequence",
                ChildIds = { "log", "wait" },
            };
            BTEditorNodeData log = new()
            {
                NodeId = "log",
                NodeKind = BTNodeKind.Action,
                NodeTypeId = BTBuiltinNodeTypes.Log,
                HandlerName = "Log",
                Title = "Log Tick",
                Arguments =
                {
                    new BTArgumentData
                    {
                        Name = "message",
                        Value = new BTSerializedValue
                        {
                            ValueType = BTValueType.String,
                            StringValue = "AITest tick",
                        },
                    },
                },
            };
            BTEditorNodeData wait = new()
            {
                NodeId = "wait",
                NodeKind = BTNodeKind.Wait,
                Title = "Wait",
                WaitMilliseconds = 1000,
            };

            asset.Nodes.Add(root);
            asset.Nodes.Add(repeat);
            asset.Nodes.Add(sequence);
            asset.Nodes.Add(log);
            asset.Nodes.Add(wait);
            asset.RootNodeId = root.NodeId;

            byte[] bytes = BTExporter.BuildBytes(asset);
            ScriptableObject.DestroyImmediate(asset);
            return bytes;
        }
    }
}
