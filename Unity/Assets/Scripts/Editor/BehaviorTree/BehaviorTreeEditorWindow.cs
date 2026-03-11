using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace ET
{
    public sealed class BehaviorTreeEditorWindow : EditorWindow
    {
        private BehaviorTreeAsset asset;
        private BehaviorTreeGraphView graphView;
        private IMGUIContainer leftPanel;
        private IMGUIContainer rightPanel;
        private BehaviorTreeNodeView selectedNodeView;
        private Vector2 leftScroll;
        private Vector2 rightScroll;
        private long selectedRuntimeId;

        [MenuItem("ET/AI/Behavior Tree Editor", false, 1007)]
        public static void ShowWindow()
        {
            Open();
        }

        public static void Open(BehaviorTreeAsset asset = null)
        {
            BehaviorTreeEditorWindow window = GetWindow<BehaviorTreeEditorWindow>(DockDefine.Types);
            window.titleContent = new GUIContent("BehaviorTree");
            if (asset != null)
            {
                window.LoadAsset(asset);
            }
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (EditorUtility.InstanceIDToObject(instanceId) is not BehaviorTreeAsset asset)
            {
                return false;
            }

            Open(asset);
            return true;
        }

        public BehaviorTreeDebugSnapshot GetActiveSnapshot()
        {
            if (this.asset == null)
            {
                return null;
            }

            if (this.selectedRuntimeId != 0)
            {
                return BehaviorTreeDebugHub.Instance.GetSnapshot(this.selectedRuntimeId);
            }

            return BehaviorTreeDebugHub.Instance.GetSnapshots(this.asset.TreeId).FirstOrDefault();
        }

        public void MarkAssetDirty()
        {
            if (this.asset == null)
            {
                return;
            }

            EditorUtility.SetDirty(this.asset);
            this.leftPanel?.MarkDirtyRepaint();
            this.rightPanel?.MarkDirtyRepaint();
        }

        public void SelectNode(BehaviorTreeNodeView nodeView)
        {
            this.selectedNodeView = nodeView;
            this.rightPanel?.MarkDirtyRepaint();
        }

        private void CreateGUI()
        {
            this.rootVisualElement.Clear();

            TwoPaneSplitView rootSplit = new(0, 320, TwoPaneSplitViewOrientation.Horizontal);
            TwoPaneSplitView workSplit = new(1, Mathf.Max((int)this.position.width - 360, 400), TwoPaneSplitViewOrientation.Horizontal);

            this.leftPanel = new IMGUIContainer(this.DrawLeftPanel);
            this.rightPanel = new IMGUIContainer(this.DrawRightPanel);
            this.graphView = new BehaviorTreeGraphView(this);

            rootSplit.Add(this.leftPanel);
            workSplit.Add(this.graphView);
            workSplit.Add(this.rightPanel);
            rootSplit.Add(workSplit);
            this.rootVisualElement.Add(rootSplit);

            if (this.asset == null && Selection.activeObject is BehaviorTreeAsset selectedAsset)
            {
                this.LoadAsset(selectedAsset);
                return;
            }

            this.graphView.PopulateView(this.asset);
        }

        private void OnSelectionChange()
        {
            if (Selection.activeObject is BehaviorTreeAsset selectedAsset)
            {
                this.LoadAsset(selectedAsset);
            }
        }

        private void Update()
        {
            this.graphView?.RefreshDebugStates(this.GetActiveSnapshot());
        }

        private void LoadAsset(BehaviorTreeAsset newAsset)
        {
            this.asset = newAsset;
            this.selectedNodeView = null;
            this.selectedRuntimeId = 0;

            if (this.asset != null)
            {
                this.asset.EnsureInitialized();
            }

            this.graphView?.PopulateView(this.asset);
            this.leftPanel?.MarkDirtyRepaint();
            this.rightPanel?.MarkDirtyRepaint();
        }

        private void DrawLeftPanel()
        {
            this.leftScroll = EditorGUILayout.BeginScrollView(this.leftScroll);
            EditorGUI.BeginChangeCheck();

            BehaviorTreeAsset selectedAsset = (BehaviorTreeAsset)EditorGUILayout.ObjectField("Asset", this.asset, typeof(BehaviorTreeAsset), false);
            if (selectedAsset != this.asset)
            {
                this.LoadAsset(selectedAsset);
                EditorGUI.EndChangeCheck();
                EditorGUILayout.EndScrollView();
                return;
            }

            if (this.asset == null)
            {
                EditorGUILayout.HelpBox("Select a BehaviorTree asset to start editing.", MessageType.Info);
                EditorGUI.EndChangeCheck();
                EditorGUILayout.EndScrollView();
                return;
            }

            EditorGUILayout.Space(8);

            this.asset.TreeName = EditorGUILayout.TextField("TreeName", this.asset.TreeName);
            EditorGUILayout.LabelField("TreeId", this.asset.TreeId);
            this.asset.Description = EditorGUILayout.TextArea(this.asset.Description, GUILayout.MinHeight(50));
            this.asset.ExportRelativePath = EditorGUILayout.TextField("Export Path", this.asset.ExportRelativePath);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
            {
                AssetDatabase.SaveAssets();
            }

            if (GUILayout.Button("Export"))
            {
                try
                {
                    string outputPath = BehaviorTreeExporter.ExportToFile(this.asset);
                    EditorUtility.DisplayDialog("BehaviorTree Export", $"Exported to:\n{outputPath}", "OK");
                }
                catch (Exception exception)
                {
                    EditorUtility.DisplayDialog("BehaviorTree Export Failed", exception.Message, "OK");
                }
            }

            if (GUILayout.Button("Reveal"))
            {
                string projectRoot = Path.GetDirectoryName(Application.dataPath) ?? string.Empty;
                string fullPath = Path.GetFullPath(Path.Combine(projectRoot, this.asset.ExportRelativePath));
                EditorUtility.RevealInFinder(fullPath);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Blackboard", EditorStyles.boldLabel);
            for (int index = 0; index < this.asset.BlackboardEntries.Count; ++index)
            {
                BehaviorTreeBlackboardEntryDefinition entry = this.asset.BlackboardEntries[index];
                EditorGUILayout.BeginVertical("box");
                entry.Key = EditorGUILayout.TextField("Key", entry.Key);
                entry.Description = EditorGUILayout.TextField("Description", entry.Description);
                entry.ValueType = (BehaviorTreeValueType)EditorGUILayout.EnumPopup("ValueType", entry.ValueType);
                DrawSerializedValueEditor("Default", entry.DefaultValue, entry.ValueType);
                if (GUILayout.Button("Remove Blackboard Entry"))
                {
                    this.asset.BlackboardEntries.RemoveAt(index);
                    --index;
                }

                EditorGUILayout.EndVertical();
            }

            if (GUILayout.Button("Add Blackboard Entry"))
            {
                this.asset.BlackboardEntries.Add(new BehaviorTreeBlackboardEntryDefinition());
            }

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Runtime Debug", EditorStyles.boldLabel);
            var snapshots = BehaviorTreeDebugHub.Instance.GetSnapshots(this.asset.TreeId);
            if (snapshots.Count == 0)
            {
                EditorGUILayout.HelpBox("No running tree instance found for this TreeId.", MessageType.None);
            }
            else
            {
                string[] runtimeOptions = snapshots.Select(snapshot => $"{snapshot.RuntimeId} / Owner:{snapshot.OwnerInstanceId}").ToArray();
                int currentIndex = Mathf.Max(0, Array.FindIndex(snapshots.ToArray(), snapshot => snapshot.RuntimeId == this.selectedRuntimeId));
                int selectedIndex = EditorGUILayout.Popup("Runtime", currentIndex, runtimeOptions);
                this.selectedRuntimeId = snapshots[selectedIndex].RuntimeId;
            }

            if (EditorGUI.EndChangeCheck())
            {
                this.MarkAssetDirty();
                this.graphView?.RefreshNodeViews();
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawRightPanel()
        {
            this.rightScroll = EditorGUILayout.BeginScrollView(this.rightScroll);
            EditorGUI.BeginChangeCheck();
            if (this.asset == null)
            {
                EditorGUI.EndChangeCheck();
                EditorGUILayout.EndScrollView();
                return;
            }

            if (this.selectedNodeView == null)
            {
                EditorGUILayout.HelpBox("Select a node to edit its properties.", MessageType.Info);
                EditorGUI.EndChangeCheck();
                EditorGUILayout.EndScrollView();
                return;
            }

            BehaviorTreeEditorNodeData node = this.selectedNodeView.Data;
            node.Title = EditorGUILayout.TextField("Title", node.Title);
            EditorGUILayout.LabelField("NodeId", node.NodeId);
            EditorGUILayout.LabelField("NodeKind", node.NodeKind.ToString());
            node.Comment = EditorGUILayout.TextArea(node.Comment, GUILayout.MinHeight(48));

            switch (node.NodeKind)
            {
                case BehaviorTreeNodeKind.Action:
                    DrawHandlerPopup("Handler", BehaviorTreeEditorUtility.GetActionHandlerNames(), node, value => node.HandlerName = value);
                    DrawArguments(node);
                    break;
                case BehaviorTreeNodeKind.Condition:
                    DrawHandlerPopup("Handler", BehaviorTreeEditorUtility.GetConditionHandlerNames(), node, value => node.HandlerName = value);
                    DrawArguments(node);
                    break;
                case BehaviorTreeNodeKind.Service:
                    DrawHandlerPopup("Handler", BehaviorTreeEditorUtility.GetServiceHandlerNames(), node, value => node.HandlerName = value);
                    node.IntervalMilliseconds = EditorGUILayout.IntField("Interval(ms)", node.IntervalMilliseconds);
                    DrawArguments(node);
                    break;
                case BehaviorTreeNodeKind.Wait:
                    node.WaitMilliseconds = EditorGUILayout.IntField("Wait(ms)", node.WaitMilliseconds);
                    break;
                case BehaviorTreeNodeKind.Repeater:
                    node.MaxLoopCount = EditorGUILayout.IntField("Loop Count", node.MaxLoopCount);
                    break;
                case BehaviorTreeNodeKind.BlackboardCondition:
                    node.BlackboardKey = EditorGUILayout.TextField("Blackboard Key", node.BlackboardKey);
                    node.CompareOperator = (BehaviorTreeCompareOperator)EditorGUILayout.EnumPopup("Operator", node.CompareOperator);
                    node.AbortMode = (BehaviorTreeAbortMode)EditorGUILayout.EnumFlagsField("Abort Mode", node.AbortMode);
                    DrawSerializedValueEditor("Compare Value", node.CompareValue);
                    break;
                case BehaviorTreeNodeKind.Parallel:
                    node.SuccessPolicy = (BehaviorTreeParallelPolicy)EditorGUILayout.EnumPopup("Success Policy", node.SuccessPolicy);
                    node.FailurePolicy = (BehaviorTreeParallelPolicy)EditorGUILayout.EnumPopup("Failure Policy", node.FailurePolicy);
                    break;
                case BehaviorTreeNodeKind.SubTree:
                    node.SubTreeAsset = (BehaviorTreeAsset)EditorGUILayout.ObjectField("SubTree Asset", node.SubTreeAsset, typeof(BehaviorTreeAsset), false);
                    node.SyncSubTreeInfo();
                    EditorGUILayout.LabelField("SubTreeId", node.SubTreeId);
                    EditorGUILayout.LabelField("SubTreeName", node.SubTreeName);
                    break;
            }

            if (EditorGUI.EndChangeCheck())
            {
                this.selectedNodeView.RefreshView(this.GetActiveSnapshot()?.NodeStates.TryGetValue(node.NodeId, out BehaviorTreeNodeState state) == true ? state : BehaviorTreeNodeState.Inactive);
                this.MarkAssetDirty();
                this.graphView?.RefreshNodeViews();
            }
            EditorGUILayout.EndScrollView();
        }

        private static void DrawArguments(BehaviorTreeEditorNodeData node)
        {
            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Arguments", EditorStyles.boldLabel);
            for (int index = 0; index < node.Arguments.Count; ++index)
            {
                BehaviorTreeArgumentDefinition argument = node.Arguments[index];
                EditorGUILayout.BeginVertical("box");
                argument.Name = EditorGUILayout.TextField("Name", argument.Name);
                DrawSerializedValueEditor("Value", argument.Value);
                if (GUILayout.Button("Remove Argument"))
                {
                    node.Arguments.RemoveAt(index);
                    --index;
                }

                EditorGUILayout.EndVertical();
            }

            if (GUILayout.Button("Add Argument"))
            {
                node.Arguments.Add(new BehaviorTreeArgumentDefinition());
            }
        }

        private static void DrawHandlerPopup(string label, string[] options, BehaviorTreeEditorNodeData node, Action<string> assign)
        {
            if (options == null || options.Length == 0)
            {
                assign(EditorGUILayout.TextField(label, node.HandlerName));
                return;
            }

            int selectedIndex = Mathf.Max(0, Array.IndexOf(options, node.HandlerName));
            int newIndex = EditorGUILayout.Popup(label, selectedIndex, options);
            assign(options[newIndex]);
        }

        private static void DrawSerializedValueEditor(string label, BehaviorTreeSerializedValue value, BehaviorTreeValueType forceType = BehaviorTreeValueType.None)
        {
            if (value == null)
            {
                return;
            }

            value.ValueType = forceType == BehaviorTreeValueType.None
                    ? (BehaviorTreeValueType)EditorGUILayout.EnumPopup(label + " Type", value.ValueType)
                    : forceType;

            switch (value.ValueType)
            {
                case BehaviorTreeValueType.Integer:
                    value.IntValue = EditorGUILayout.IntField(label, value.IntValue);
                    break;
                case BehaviorTreeValueType.Long:
                    value.LongValue = EditorGUILayout.LongField(label, value.LongValue);
                    break;
                case BehaviorTreeValueType.Float:
                    value.FloatValue = EditorGUILayout.FloatField(label, value.FloatValue);
                    break;
                case BehaviorTreeValueType.Boolean:
                    value.BoolValue = EditorGUILayout.Toggle(label, value.BoolValue);
                    break;
                case BehaviorTreeValueType.String:
                    value.StringValue = EditorGUILayout.TextField(label, value.StringValue);
                    break;
            }
        }
    }
}
