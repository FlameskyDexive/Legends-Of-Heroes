using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ET
{
    public sealed class BehaviorTreeEditorWindow : EditorWindow
    {
        private const string MiniMapEditorPrefKey = "ET.BehaviorTreeEditor.ShowMiniMap";
        private const string GridEditorPrefKey = "ET.BehaviorTreeEditor.ShowGrid";
        private const string ConnectionStyleEditorPrefKey = "ET.BehaviorTreeEditor.ConnectionStyle";
        private const string MiniMapRectEditorPrefKey = "ET.BehaviorTreeEditor.MiniMapRect";

        private BehaviorTreeAsset asset;
        private Toolbar toolbar;
        private BehaviorTreeGraphView graphView;
        private ToolbarMenu assetMenu;
        private IMGUIContainer inspectorPanel;
        private IMGUIContainer blackboardPanel;
        private BehaviorTreeNodeView selectedNodeView;
        private Vector2 inspectorScroll;
        private Vector2 blackboardScroll;
        private string blackboardSearchText = string.Empty;
        private long selectedRuntimeId;
        private long lastSnapshotRuntimeId;
        private long lastSnapshotUpdatedAt;
        private bool showMiniMap = true;
        private bool showGrid = true;
        private BehaviorTreeConnectionStyle connectionStyle = BehaviorTreeConnectionStyle.Orthogonal;

        [MenuItem("ET/AI/BehaviorTreeEditor", false, 1007)]
        public static void ShowWindow()
        {
            Open(GetSelectedAsset());
        }

        public static void Open(BehaviorTreeAsset asset = null)
        {
            BehaviorTreeEditorWindow window = GetWindow<BehaviorTreeEditorWindow>(DockDefine.Types);
            window.titleContent = new GUIContent("BehaviourTreeEditor");
            BehaviorTreeAsset selectedAsset = asset ?? GetSelectedAsset();
            if (selectedAsset != null)
            {
                window.LoadAsset(selectedAsset);
            }
            window.Focus();
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

        private static BehaviorTreeAsset GetSelectedAsset()
        {
            if (Selection.activeObject is BehaviorTreeAsset behaviorTreeAsset)
            {
                return behaviorTreeAsset;
            }

            foreach (UnityEngine.Object selectedObject in Selection.objects)
            {
                if (selectedObject is BehaviorTreeAsset asset)
                {
                    return asset;
                }
            }

            return null;
        }

        public BehaviorTreeDebugSnapshot GetActiveSnapshot()
        {
            if (this.asset == null)
            {
                return null;
            }

            BehaviorTreeDebugHub debugHub = BehaviorTreeDebugHub.Instance;
            if (debugHub == null)
            {
                return null;
            }

            if (this.selectedRuntimeId != 0)
            {
                return debugHub.GetSnapshot(this.selectedRuntimeId);
            }

            return debugHub.GetSnapshots(this.asset.TreeId).FirstOrDefault();
        }

        public BehaviorTreeRunner GetActiveRunner()
        {
            long runtimeId = this.GetActiveSnapshot()?.RuntimeId ?? 0;
            if (runtimeId == 0)
            {
                return null;
            }

            BehaviorTreeRuntimeManager runtimeManager = BehaviorTreeRuntimeManager.Instance;
            return runtimeManager?.Get(runtimeId);
        }

        public void MarkAssetDirty()
        {
            if (this.asset == null)
            {
                return;
            }

            EditorUtility.SetDirty(this.asset);
            this.inspectorPanel?.MarkDirtyRepaint();
            this.blackboardPanel?.MarkDirtyRepaint();
        }

        public void SelectNode(BehaviorTreeNodeView nodeView)
        {
            this.selectedNodeView = nodeView;
            this.inspectorPanel?.MarkDirtyRepaint();
        }

        public void OpenNodeScript(BehaviorTreeNodeView nodeView)
        {
            if (nodeView?.Data == null)
            {
                return;
            }

            this.SelectNode(nodeView);
            if (BehaviorTreeEditorUtility.TryOpenNodeScript(nodeView.Data))
            {
                return;
            }

            this.ShowNotification(new GUIContent($"No handler script found for node: {nodeView.Data.Title}"));
        }

        private void OnEnable()
        {
            this.showMiniMap = EditorPrefs.GetBool(MiniMapEditorPrefKey, true);
            this.showGrid = EditorPrefs.GetBool(GridEditorPrefKey, true);
            this.connectionStyle = (BehaviorTreeConnectionStyle)EditorPrefs.GetInt(ConnectionStyleEditorPrefKey, (int)BehaviorTreeConnectionStyle.Orthogonal);
        }

        private void CreateGUI()
        {
            this.rootVisualElement.Clear();
            this.rootVisualElement.style.flexDirection = FlexDirection.Column;
            this.rootVisualElement.style.flexGrow = 1;

            this.rootVisualElement.Add(this.CreateToolbar());

            VisualElement mainContainer = new();
            mainContainer.style.flexDirection = FlexDirection.Row;
            mainContainer.style.flexGrow = 1;
            mainContainer.style.backgroundColor = new Color(0.14f, 0.14f, 0.14f);

            VisualElement leftContainer = new();
            leftContainer.style.width = 320;
            leftContainer.style.minWidth = 280;
            leftContainer.style.flexDirection = FlexDirection.Column;
            leftContainer.style.flexShrink = 0;
            leftContainer.style.backgroundColor = new Color(0.18f, 0.18f, 0.18f);
            leftContainer.style.borderRightWidth = 2;
            leftContainer.style.borderRightColor = new Color(0.10f, 0.10f, 0.10f);

            VisualElement rightContainer = new();
            rightContainer.style.flexGrow = 1;
            rightContainer.style.flexDirection = FlexDirection.Column;
            rightContainer.style.backgroundColor = new Color(0.12f, 0.12f, 0.12f);

            this.inspectorPanel = new IMGUIContainer(this.DrawInspectorPanel);
            this.inspectorPanel.style.flexGrow = 1;
            this.blackboardPanel = new IMGUIContainer(this.DrawBlackboardPanel);
            this.blackboardPanel.style.flexGrow = 1;

            VisualElement inspectorContainer = this.CreatePanel("Inspector", this.inspectorPanel);
            inspectorContainer.style.flexGrow = 1;
            inspectorContainer.style.minHeight = 260;

            VisualElement blackboardContainer = this.CreatePanel("Blackboard", this.blackboardPanel);
            blackboardContainer.style.flexGrow = 1;
            blackboardContainer.style.minHeight = 220;

            leftContainer.Add(inspectorContainer);
            leftContainer.Add(blackboardContainer);

            try
            {
                this.graphView = new BehaviorTreeGraphView(this);
            }
            catch (Exception exception)
            {
                Label errorLabel = new($"BehaviorTreeGraphView init failed:\n{exception}");
                errorLabel.style.whiteSpace = WhiteSpace.Normal;
                errorLabel.style.color = new Color(1f, 0.45f, 0.45f);
                errorLabel.style.paddingLeft = 12;
                errorLabel.style.paddingTop = 12;
                rightContainer.Add(errorLabel);
                this.rootVisualElement.Add(mainContainer);
                return;
            }

            this.graphView.SetMiniMapVisible(this.showMiniMap);
            this.graphView.SetGridVisible(this.showGrid);
            this.graphView.SetConnectionStyle(this.connectionStyle);
            VisualElement treeViewPanel = this.CreatePanel("Tree View", this.graphView);
            treeViewPanel.style.flexGrow = 1;

            rightContainer.Add(treeViewPanel);

            mainContainer.Add(leftContainer);
            mainContainer.Add(rightContainer);
            this.rootVisualElement.Add(mainContainer);

            this.PopulateAssetMenu();

            if (this.asset == null && Selection.activeObject is BehaviorTreeAsset selectedAsset)
            {
                this.LoadAsset(selectedAsset);
                this.graphView?.Focus();
                return;
            }

            this.graphView.PopulateView(this.asset);
            this.graphView?.FrameAllNodes();
            this.graphView?.Focus();
        }

        private void OnFocus()
        {
            this.PopulateAssetMenu();
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
            BehaviorTreeDebugSnapshot snapshot = this.GetActiveSnapshot();
            this.graphView?.RefreshDebugStates(snapshot);

            long runtimeId = snapshot?.RuntimeId ?? 0;
            long updatedAt = snapshot?.UpdatedAt ?? 0;
            if (runtimeId != this.lastSnapshotRuntimeId || updatedAt != this.lastSnapshotUpdatedAt)
            {
                this.lastSnapshotRuntimeId = runtimeId;
                this.lastSnapshotUpdatedAt = updatedAt;
                this.blackboardPanel?.MarkDirtyRepaint();
                this.inspectorPanel?.MarkDirtyRepaint();
            }

            if (this.GetActiveRunner() != null)
            {
                this.blackboardPanel?.MarkDirtyRepaint();
            }
        }

        private Toolbar CreateToolbar()
        {
            this.toolbar = new Toolbar();

            this.assetMenu = this.CreateAssetMenu();
            this.toolbar.Add(this.assetMenu);

            this.toolbar.Add(this.CreateToolbarButton("New", BehaviorTreeAsset.CreateAsset));
            this.toolbar.Add(this.CreateToolbarButton("Save", () => AssetDatabase.SaveAssets()));
            this.toolbar.Add(this.CreateToolbarButton("Export", this.ExportCurrentAsset));
            this.toolbar.Add(this.CreateToolbarButton("Reveal", this.RevealCurrentAsset));
            this.toolbar.Add(this.CreateToolbarButton("Frame", () => this.graphView?.FrameAllNodes()));
            this.toolbar.Add(this.CreateToolbarButton("Layout", () => this.graphView?.AutoLayoutTree()));
            this.toolbar.Add(this.CreateToolbarButton("Refresh", this.RefreshToolbar));

            ToolbarMenu arrangeMenu = new()
            {
                text = "Arrange",
            };
            arrangeMenu.menu.AppendAction("Align Left", _ => this.graphView?.AlignSelectionLeft());
            arrangeMenu.menu.AppendAction("Align Right", _ => this.graphView?.AlignSelectionRight());
            arrangeMenu.menu.AppendAction("Align Top", _ => this.graphView?.AlignSelectionTop());
            arrangeMenu.menu.AppendAction("Align Bottom", _ => this.graphView?.AlignSelectionBottom());
            arrangeMenu.menu.AppendAction("Align Horizontal Center", _ => this.graphView?.AlignSelectionHorizontalCenter());
            arrangeMenu.menu.AppendAction("Align Vertical Center", _ => this.graphView?.AlignSelectionVerticalCenter());
            arrangeMenu.menu.AppendSeparator();
            arrangeMenu.menu.AppendAction("Distribute Horizontal", _ => this.graphView?.DistributeSelectionHorizontal());
            arrangeMenu.menu.AppendAction("Distribute Vertical", _ => this.graphView?.DistributeSelectionVertical());
            this.toolbar.Add(arrangeMenu);

            ToolbarToggle miniMapToggle = new()
            {
                text = "MiniMap",
                value = this.showMiniMap,
            };
            miniMapToggle.RegisterValueChangedCallback(evt => this.SetMiniMapVisible(evt.newValue));
            this.toolbar.Add(miniMapToggle);
            this.toolbar.Add(this.CreateToolbarButton("Reset MiniMap", this.ResetMiniMap));

            ToolbarToggle gridToggle = new()
            {
                text = "Grid",
                value = this.showGrid,
            };
            gridToggle.RegisterValueChangedCallback(evt => this.SetGridVisible(evt.newValue));
            this.toolbar.Add(gridToggle);

            ToolbarMenu lineMenu = new()
            {
                text = this.GetConnectionStyleLabel(),
            };
            lineMenu.menu.AppendAction("Straight", _ => this.SetConnectionStyle(BehaviorTreeConnectionStyle.Straight),
                _ => this.connectionStyle == BehaviorTreeConnectionStyle.Straight ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
            lineMenu.menu.AppendAction("Orthogonal", _ => this.SetConnectionStyle(BehaviorTreeConnectionStyle.Orthogonal),
                _ => this.connectionStyle == BehaviorTreeConnectionStyle.Orthogonal ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
            lineMenu.menu.AppendAction("Curve", _ => this.SetConnectionStyle(BehaviorTreeConnectionStyle.Curve),
                _ => this.connectionStyle == BehaviorTreeConnectionStyle.Curve ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
            this.toolbar.Add(lineMenu);

            VisualElement spacer = new();
            spacer.style.flexGrow = 1;
            this.toolbar.Add(spacer);

            Label selectionLabel = new();
            selectionLabel.name = "SelectionLabel";
            selectionLabel.style.unityTextAlign = TextAnchor.MiddleRight;
            selectionLabel.style.marginRight = 8;
            selectionLabel.text = this.asset == null ? "No Tree Selected" : this.asset.TreeName;
            this.toolbar.Add(selectionLabel);

            return this.toolbar;
        }

        private void SetMiniMapVisible(bool visible)
        {
            this.showMiniMap = visible;
            EditorPrefs.SetBool(MiniMapEditorPrefKey, visible);
            this.graphView?.SetMiniMapVisible(visible);
        }

        private void ResetMiniMap()
        {
            Rect rect = new Rect(12, 36, 220, 140);
            SaveMiniMapRect(rect);
            this.graphView?.SetMiniMapRect(rect);
        }

        public Rect LoadMiniMapRect()
        {
            string savedValue = EditorPrefs.GetString(MiniMapRectEditorPrefKey, string.Empty);
            if (string.IsNullOrWhiteSpace(savedValue))
            {
                return new Rect(12, 36, 220, 140);
            }

            string[] values = savedValue.Split('|');
            if (values.Length != 4)
            {
                return new Rect(12, 36, 220, 140);
            }

            if (!float.TryParse(values[0], out float x) || !float.TryParse(values[1], out float y) ||
                !float.TryParse(values[2], out float width) || !float.TryParse(values[3], out float height))
            {
                return new Rect(12, 36, 220, 140);
            }

            return new Rect(x, y, width, height);
        }

        public void SaveMiniMapRect(Rect rect)
        {
            string value = $"{rect.x}|{rect.y}|{rect.width}|{rect.height}";
            EditorPrefs.SetString(MiniMapRectEditorPrefKey, value);
        }

        private void SetGridVisible(bool visible)
        {
            this.showGrid = visible;
            EditorPrefs.SetBool(GridEditorPrefKey, visible);
            this.graphView?.SetGridVisible(visible);
        }

        private void SetConnectionStyle(BehaviorTreeConnectionStyle style)
        {
            this.connectionStyle = style;
            EditorPrefs.SetInt(ConnectionStyleEditorPrefKey, (int)style);
            this.graphView?.SetConnectionStyle(style);

            ToolbarMenu lineMenu = this.toolbar?.Children().OfType<ToolbarMenu>().FirstOrDefault(menu => menu.text.StartsWith("Line:"));
            if (lineMenu != null)
            {
                lineMenu.text = this.GetConnectionStyleLabel();
            }
        }

        private string GetConnectionStyleLabel()
        {
            return this.connectionStyle switch
            {
                BehaviorTreeConnectionStyle.Straight => "Line: Straight",
                BehaviorTreeConnectionStyle.Curve => "Line: Curve",
                _ => "Line: Orthogonal",
            };
        }

        private Button CreateToolbarButton(string text, Action onClick)
        {
            Button button = new(onClick)
            {
                text = text,
            };
            return button;
        }

        private VisualElement CreatePanel(string title, VisualElement content)
        {
            VisualElement panel = new();
            panel.style.flexDirection = FlexDirection.Column;
            panel.style.flexGrow = 1;
            panel.style.borderBottomWidth = 1;
            panel.style.borderLeftWidth = 1;
            panel.style.borderRightWidth = 1;
            panel.style.borderTopWidth = 1;
            panel.style.borderBottomColor = new Color(0.15f, 0.15f, 0.15f);
            panel.style.borderLeftColor = new Color(0.15f, 0.15f, 0.15f);
            panel.style.borderRightColor = new Color(0.15f, 0.15f, 0.15f);
            panel.style.borderTopColor = new Color(0.15f, 0.15f, 0.15f);

            Label header = new(title);
            header.style.unityFontStyleAndWeight = FontStyle.Bold;
            header.style.paddingLeft = 8;
            header.style.paddingTop = 4;
            header.style.paddingBottom = 4;
            header.style.backgroundColor = new Color(0.18f, 0.18f, 0.18f);
            panel.Add(header);

            content.style.flexGrow = 1;
            panel.Add(content);
            return panel;
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

            this.PopulateAssetMenu();
            this.graphView?.PopulateView(this.asset);
            this.inspectorPanel?.MarkDirtyRepaint();
            this.blackboardPanel?.MarkDirtyRepaint();

            Label selectionLabel = this.rootVisualElement.Q<Label>("SelectionLabel");
            if (selectionLabel != null)
            {
                selectionLabel.text = this.asset == null ? "No Tree Selected" : this.asset.TreeName;
            }
        }

        private void PopulateAssetMenu()
        {
            if (this.toolbar == null || this.assetMenu == null)
            {
                return;
            }

            ToolbarMenu newAssetMenu = this.CreateAssetMenu();
            int index = this.toolbar.IndexOf(this.assetMenu);
            if (index < 0)
            {
                index = 0;
            }

            this.toolbar.Remove(this.assetMenu);
            this.assetMenu = newAssetMenu;
            this.toolbar.Insert(index, this.assetMenu);
        }

        private ToolbarMenu CreateAssetMenu()
        {
            ToolbarMenu menu = new();
            menu.text = this.asset == null ? "Assets" : this.asset.TreeName;

            string[] guids = AssetDatabase.FindAssets("t:BehaviorTreeAsset");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                BehaviorTreeAsset behaviorTreeAsset = AssetDatabase.LoadAssetAtPath<BehaviorTreeAsset>(path);
                if (behaviorTreeAsset == null)
                {
                    continue;
                }

                menu.menu.AppendAction(path, _ => this.LoadAsset(behaviorTreeAsset),
                    _ => this.asset == behaviorTreeAsset ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
            }

            menu.menu.AppendSeparator();
            menu.menu.AppendAction("Refresh Assets", _ => this.RefreshToolbar());
            return menu;
        }

        private void RefreshToolbar()
        {
            this.PopulateAssetMenu();
            this.graphView?.RefreshNodeViews();
            this.inspectorPanel?.MarkDirtyRepaint();
            this.blackboardPanel?.MarkDirtyRepaint();
        }

        private void DrawInspectorPanel()
        {
            this.inspectorScroll = EditorGUILayout.BeginScrollView(this.inspectorScroll);
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
                EditorGUILayout.HelpBox("Select a behavior tree asset from the Assets menu or Project window.", MessageType.Info);
                EditorGUI.EndChangeCheck();
                EditorGUILayout.EndScrollView();
                return;
            }

            if (this.selectedNodeView == null)
            {
                this.DrawTreeInspector();
            }
            else
            {
                this.DrawNodeInspector(this.selectedNodeView.Data);
            }

            if (EditorGUI.EndChangeCheck())
            {
                this.MarkAssetDirty();
                this.graphView?.RefreshNodeViews();
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawTreeInspector()
        {
            EditorGUILayout.LabelField("Script", this.asset.GetType().Name);
            this.asset.TreeName = EditorGUILayout.TextField("Tree Name", this.asset.TreeName);
            EditorGUILayout.LabelField("Tree Id", this.asset.TreeId);
            EditorGUILayout.LabelField("Client Bytes", this.asset.ExportRelativePath);
            EditorGUILayout.LabelField("Server Bytes", Path.Combine("Config/AI", Path.GetFileName(this.asset.ExportRelativePath)));
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Description");
            this.asset.Description = EditorGUILayout.TextArea(this.asset.Description, GUILayout.MinHeight(70));
            EditorGUILayout.Space(8);

            if (GUILayout.Button("Select Root Node"))
            {
                this.graphView?.SelectNode(this.asset.RootNodeId);
            }

            BehaviorTreeDebugSnapshot snapshot = this.GetActiveSnapshot();
            if (snapshot != null)
            {
                EditorGUILayout.Space(8);
                EditorGUILayout.LabelField("Runtime", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Runtime Id", snapshot.RuntimeId.ToString());
                EditorGUILayout.LabelField("Updated At", snapshot.UpdatedAt.ToString());
                EditorGUILayout.LabelField("Active Nodes", snapshot.NodeStates.Count.ToString());
            }
        }

        private void DrawNodeInspector(BehaviorTreeEditorNodeData node)
        {
            BehaviorTreeEditorUtility.SyncNodeDescriptor(node);
            EditorGUILayout.LabelField("Script", BehaviorTreeEditorUtility.GetNodeScriptName(node));
            node.Title = EditorGUILayout.TextField("Title", node.Title);
            EditorGUILayout.LabelField("Node Id", node.NodeId);
            if (GUILayout.Button("Open Script"))
            {
                this.OpenNodeScript(this.selectedNodeView);
            }
            EditorGUILayout.LabelField("Description");
            node.Comment = EditorGUILayout.TextArea(node.Comment, GUILayout.MinHeight(56));
            EditorGUILayout.Space(6);

            switch (node.NodeKind)
            {
                case BehaviorTreeNodeKind.Action:
                    if (BehaviorTreeEditorUtility.TryGetDescriptor(node, out ABehaviorTreeNodeDescriptor actionDescriptor))
                    {
                        this.DrawDescriptorNode(node, actionDescriptor);
                        if (string.Equals(node.NodeTypeId, ET.Client.BehaviorTreeDemoNodeTypes.Patrol, StringComparison.OrdinalIgnoreCase))
                        {
                            this.DrawPatrolPointsEditor(node);
                        }
                    }
                    else
                    {
                        DrawHandlerPopup("Handler", BehaviorTreeEditorUtility.GetActionHandlerNames(), node, value => node.HandlerName = value);
                        DrawArguments(node);
                    }
                    break;
                case BehaviorTreeNodeKind.Condition:
                    if (BehaviorTreeEditorUtility.TryGetDescriptor(node, out ABehaviorTreeNodeDescriptor conditionDescriptor))
                    {
                        this.DrawDescriptorNode(node, conditionDescriptor);
                    }
                    else
                    {
                        DrawHandlerPopup("Handler", BehaviorTreeEditorUtility.GetConditionHandlerNames(), node, value => node.HandlerName = value);
                        DrawArguments(node);
                    }
                    break;
                case BehaviorTreeNodeKind.Service:
                    if (BehaviorTreeEditorUtility.TryGetDescriptor(node, out ABehaviorTreeNodeDescriptor serviceDescriptor))
                    {
                        this.DrawDescriptorNode(node, serviceDescriptor, true);
                    }
                    else
                    {
                        DrawHandlerPopup("Handler", BehaviorTreeEditorUtility.GetServiceHandlerNames(), node, value => node.HandlerName = value);
                        node.IntervalMilliseconds = EditorGUILayout.IntField("Interval (ms)", node.IntervalMilliseconds);
                        DrawArguments(node);
                    }
                    break;
                case BehaviorTreeNodeKind.Wait:
                    node.WaitMilliseconds = EditorGUILayout.IntField("Delay (ms)", node.WaitMilliseconds);
                    break;
                case BehaviorTreeNodeKind.Repeater:
                    node.MaxLoopCount = EditorGUILayout.IntField("Repeat Count", node.MaxLoopCount);
                    break;
                case BehaviorTreeNodeKind.BlackboardCondition:
                    DrawBlackboardKeyPopup(node);
                    node.CompareOperator = (BehaviorTreeCompareOperator)EditorGUILayout.EnumPopup("Operator", node.CompareOperator);
                    node.AbortMode = (BehaviorTreeAbortMode)EditorGUILayout.EnumFlagsField("Abort Mode", node.AbortMode);
                    DrawSerializedValueEditor("Compare Value", node.CompareValue);
                    break;
                case BehaviorTreeNodeKind.Parallel:
                    node.SuccessPolicy = (BehaviorTreeParallelPolicy)EditorGUILayout.EnumPopup("Success Policy", node.SuccessPolicy);
                    node.FailurePolicy = (BehaviorTreeParallelPolicy)EditorGUILayout.EnumPopup("Failure Policy", node.FailurePolicy);
                    break;
                case BehaviorTreeNodeKind.SubTree:
                    node.SubTreeAsset = (BehaviorTreeAsset)EditorGUILayout.ObjectField("SubTree", node.SubTreeAsset, typeof(BehaviorTreeAsset), false);
                    node.SyncSubTreeInfo();
                    EditorGUILayout.LabelField("SubTree Id", node.SubTreeId);
                    EditorGUILayout.LabelField("SubTree Name", node.SubTreeName);
                    break;
            }

            if (GUILayout.Button("Deselect Node"))
            {
                this.selectedNodeView = null;
            }
        }

        private void DrawBlackboardPanel()
        {
            this.blackboardScroll = EditorGUILayout.BeginScrollView(this.blackboardScroll);
            EditorGUI.BeginChangeCheck();

            if (this.asset == null)
            {
                EditorGUILayout.HelpBox("Blackboard values will appear here once a tree asset is selected.", MessageType.Info);
                EditorGUI.EndChangeCheck();
                EditorGUILayout.EndScrollView();
                return;
            }

            this.blackboardSearchText = EditorGUILayout.TextField("Search", this.blackboardSearchText);

            for (int index = 0; index < this.asset.BlackboardEntries.Count; ++index)
            {
                BehaviorTreeBlackboardEntryDefinition entry = this.asset.BlackboardEntries[index];
                if (!this.IsBlackboardEntryVisible(entry?.Key))
                {
                    continue;
                }

                EditorGUILayout.BeginVertical("box");
                entry.Key = EditorGUILayout.TextField("Key", entry.Key);
                entry.Description = EditorGUILayout.TextField("Description", entry.Description);
                entry.ValueType = (BehaviorTreeValueType)EditorGUILayout.EnumPopup("Type", entry.ValueType);
                DrawSerializedValueEditor("Value", entry.DefaultValue, entry.ValueType);
                if (GUILayout.Button("Remove"))
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
            BehaviorTreeDebugHub debugHub = BehaviorTreeDebugHub.Instance;
            List<BehaviorTreeDebugSnapshot> snapshots = debugHub != null
                    ? debugHub.GetSnapshots(this.asset.TreeId)
                    : new List<BehaviorTreeDebugSnapshot>();
            if (snapshots.Count == 0)
            {
                EditorGUILayout.HelpBox("No running tree instance found for this tree.", MessageType.None);
            }
            else
            {
                string[] runtimeOptions = snapshots.Select(snapshot => $"{snapshot.RuntimeId} / Owner:{snapshot.OwnerInstanceId}").ToArray();
                int currentIndex = Mathf.Max(0, Array.FindIndex(snapshots.ToArray(), snapshot => snapshot.RuntimeId == this.selectedRuntimeId));
                int selectedIndex = EditorGUILayout.Popup("Runtime", currentIndex, runtimeOptions);
                this.selectedRuntimeId = snapshots[selectedIndex].RuntimeId;
            }

            BehaviorTreeRunner runner = this.GetActiveRunner();
            if (runner != null)
            {
                EditorGUILayout.Space(8);
                EditorGUILayout.LabelField("Runtime Blackboard", EditorStyles.boldLabel);
                this.DrawRuntimeBlackboard(runner.Blackboard);
            }

            if (EditorGUI.EndChangeCheck())
            {
                this.MarkAssetDirty();
                this.graphView?.RefreshNodeViews();
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawRuntimeBlackboard(BehaviorTreeBlackboard blackboard)
        {
            if (blackboard == null)
            {
                EditorGUILayout.HelpBox("Runtime blackboard is not available.", MessageType.None);
                return;
            }

            HashSet<string> drawnKeys = new(StringComparer.OrdinalIgnoreCase);
            foreach (BehaviorTreeBlackboardEntryDefinition entry in this.asset.BlackboardEntries)
            {
                if (entry == null || string.IsNullOrWhiteSpace(entry.Key))
                {
                    continue;
                }

                drawnKeys.Add(entry.Key);
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField(entry.Key, EditorStyles.boldLabel);
                object currentValue = blackboard.GetBoxed(entry.Key) ?? BehaviorTreeValueUtility.GetValue(entry.DefaultValue);
                object newValue = DrawRuntimeValueField(entry.ValueType, currentValue);
                if (!Equals(currentValue, newValue))
                {
                    blackboard.SetBoxed(entry.Key, newValue);
                }

                if (!string.IsNullOrWhiteSpace(entry.Description))
                {
                    EditorGUILayout.LabelField(entry.Description, EditorStyles.wordWrappedMiniLabel);
                }
                EditorGUILayout.EndVertical();
            }

            List<KeyValuePair<string, object>> extraValues = blackboard.Values.Where(valuePair => !drawnKeys.Contains(valuePair.Key)).ToList();
            if (extraValues.Count > 0)
            {
                EditorGUILayout.Space(4);
                EditorGUILayout.LabelField("Extra Runtime Keys", EditorStyles.boldLabel);
                foreach ((string key, object value) in extraValues)
                {
                    if (!this.IsBlackboardEntryVisible(key))
                    {
                        continue;
                    }

                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.LabelField(key, EditorStyles.boldLabel);
                    EditorGUILayout.LabelField(value?.ToString() ?? "null");
                    EditorGUILayout.EndVertical();
                }
            }
        }

        private bool IsBlackboardEntryVisible(string key)
        {
            if (string.IsNullOrWhiteSpace(this.blackboardSearchText))
            {
                return true;
            }

            return !string.IsNullOrWhiteSpace(key) && key.IndexOf(this.blackboardSearchText, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static object DrawRuntimeValueField(BehaviorTreeValueType valueType, object currentValue)
        {
            switch (valueType)
            {
                case BehaviorTreeValueType.Integer:
                    return EditorGUILayout.IntField("Value", currentValue is int intValue ? intValue : 0);
                case BehaviorTreeValueType.Long:
                    return EditorGUILayout.LongField("Value", currentValue is long longValue ? longValue : 0L);
                case BehaviorTreeValueType.Float:
                    return EditorGUILayout.FloatField("Value", currentValue is float floatValue ? floatValue : 0f);
                case BehaviorTreeValueType.Boolean:
                    return EditorGUILayout.Toggle("Value", currentValue is bool boolValue && boolValue);
                case BehaviorTreeValueType.String:
                    return EditorGUILayout.TextField("Value", currentValue?.ToString() ?? string.Empty);
                default:
                    EditorGUILayout.LabelField("Value", currentValue?.ToString() ?? "null");
                    return currentValue;
            }
        }

        private void DrawBlackboardKeyPopup(BehaviorTreeEditorNodeData node)
        {
            string[] options = this.GetBlackboardKeyOptions();
            if (options.Length == 0)
            {
                node.BlackboardKey = EditorGUILayout.TextField("Key", node.BlackboardKey);
                return;
            }

            int selectedIndex = Mathf.Max(0, Array.IndexOf(options, node.BlackboardKey));
            int newIndex = EditorGUILayout.Popup("Key", selectedIndex, options);
            node.BlackboardKey = options[newIndex];
        }

        private string[] GetBlackboardKeyOptions()
        {
            if (this.asset == null || this.asset.BlackboardEntries.Count == 0)
            {
                return Array.Empty<string>();
            }

            return this.asset.BlackboardEntries
                    .Where(entry => !string.IsNullOrWhiteSpace(entry.Key))
                    .Select(entry => entry.Key)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(key => key, StringComparer.OrdinalIgnoreCase)
                    .ToArray();
        }

        private void ExportCurrentAsset()
        {
            if (this.asset == null)
            {
                return;
            }

            try
            {
                BehaviorTreeExporter.BehaviorTreeExportResult result = BehaviorTreeExporter.ExportToFiles(this.asset);
                EditorUtility.DisplayDialog("BehaviorTree Export",
                    $"Client:\n{result.ClientFullPath}\n\nServer:\n{result.ServerFullPath}",
                    "OK");
            }
            catch (Exception exception)
            {
                EditorUtility.DisplayDialog("BehaviorTree Export Failed", exception.Message, "OK");
            }
        }

        private void RevealCurrentAsset()
        {
            if (this.asset == null)
            {
                return;
            }

            string projectRoot = Path.GetDirectoryName(Application.dataPath) ?? string.Empty;
            string fullPath = Path.GetFullPath(Path.Combine(projectRoot, this.asset.ExportRelativePath));
            EditorUtility.RevealInFinder(fullPath);
        }

        private void DrawDescriptorNode(BehaviorTreeEditorNodeData node, ABehaviorTreeNodeDescriptor descriptor, bool drawInterval = false)
        {
            if (descriptor == null)
            {
                return;
            }

            EditorGUILayout.LabelField("Node Type", descriptor.Title);
            EditorGUILayout.LabelField("Type Id", descriptor.TypeId);
            if (!string.IsNullOrWhiteSpace(descriptor.HandlerName))
            {
                EditorGUILayout.LabelField("Handler", descriptor.HandlerName);
            }

            if (!string.IsNullOrWhiteSpace(descriptor.Description))
            {
                EditorGUILayout.HelpBox(descriptor.Description, MessageType.None);
            }

            if (drawInterval)
            {
                node.IntervalMilliseconds = EditorGUILayout.IntField("Interval (ms)", node.IntervalMilliseconds);
            }

            this.DrawDescriptorArguments(node, descriptor.Parameters);
        }

        private void DrawPatrolPointsEditor(BehaviorTreeEditorNodeData node)
        {
            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Patrol Points", EditorStyles.boldLabel);
            for (int index = 0; index < node.PatrolPoints.Count; ++index)
            {
                ET.Client.BehaviorTreePatrolPointDefinition patrolPoint = node.PatrolPoints[index] ?? new ET.Client.BehaviorTreePatrolPointDefinition();
                node.PatrolPoints[index] = patrolPoint;

                EditorGUILayout.BeginVertical("box");
                Vector3 value = new(patrolPoint.X, patrolPoint.Y, patrolPoint.Z);
                value = EditorGUILayout.Vector3Field($"Point {index + 1}", value);
                patrolPoint.X = value.x;
                patrolPoint.Y = value.y;
                patrolPoint.Z = value.z;

                if (GUILayout.Button("Remove Point"))
                {
                    node.PatrolPoints.RemoveAt(index);
                    --index;
                }

                EditorGUILayout.EndVertical();
            }

            if (GUILayout.Button("Add Patrol Point"))
            {
                node.PatrolPoints.Add(new ET.Client.BehaviorTreePatrolPointDefinition());
            }
        }

        private void DrawDescriptorArguments(BehaviorTreeEditorNodeData node, IReadOnlyList<BehaviorTreeNodeParameterDefinition> parameters)
        {
            if (parameters == null || parameters.Count == 0)
            {
                return;
            }

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
            foreach (BehaviorTreeNodeParameterDefinition parameter in parameters)
            {
                if (parameter == null || string.IsNullOrWhiteSpace(parameter.Name))
                {
                    continue;
                }

                EditorGUILayout.BeginVertical("box");
                this.DrawDescriptorArgument(node, parameter);
                if (!string.IsNullOrWhiteSpace(parameter.Description))
                {
                    EditorGUILayout.LabelField(parameter.Description, EditorStyles.wordWrappedMiniLabel);
                }

                EditorGUILayout.EndVertical();
            }
        }

        private void DrawDescriptorArgument(BehaviorTreeEditorNodeData node, BehaviorTreeNodeParameterDefinition parameter)
        {
            BehaviorTreeArgumentDefinition argument = BehaviorTreeEditorUtility.GetOrCreateArgument(node, parameter);
            argument.Value ??= parameter.DefaultValue?.Clone() ?? new BehaviorTreeSerializedValue();

            string label = string.IsNullOrWhiteSpace(parameter.DisplayName) ? parameter.Name : parameter.DisplayName;
            switch (parameter.EditorHint)
            {
                case BehaviorTreeNodeParameterEditorHint.BlackboardKey:
                    this.DrawBlackboardKeyArgument(label, argument);
                    break;
                case BehaviorTreeNodeParameterEditorHint.CompareOperator:
                    argument.Value.ValueType = BehaviorTreeValueType.Integer;
                    BehaviorTreeCompareOperator compareOperator = (BehaviorTreeCompareOperator)argument.Value.IntValue;
                    compareOperator = (BehaviorTreeCompareOperator)EditorGUILayout.EnumPopup(label, compareOperator);
                    argument.Value.IntValue = (int)compareOperator;
                    break;
                case BehaviorTreeNodeParameterEditorHint.MultilineText:
                    argument.Value.ValueType = BehaviorTreeValueType.String;
                    EditorGUILayout.LabelField(label);
                    argument.Value.StringValue = EditorGUILayout.TextArea(argument.Value.StringValue, GUILayout.MinHeight(56));
                    break;
                default:
                    DrawSerializedValueEditor(label, argument.Value, parameter.ValueType);
                    break;
            }
        }

        private void DrawBlackboardKeyArgument(string label, BehaviorTreeArgumentDefinition argument)
        {
            argument.Value ??= new BehaviorTreeSerializedValue();
            argument.Value.ValueType = BehaviorTreeValueType.String;
            string[] options = this.GetBlackboardKeyOptions();
            if (options.Length == 0)
            {
                argument.Value.StringValue = EditorGUILayout.TextField(label, argument.Value.StringValue);
                return;
            }

            int selectedIndex = Array.IndexOf(options, argument.Value.StringValue);
            int newIndex = EditorGUILayout.Popup(label, Mathf.Max(0, selectedIndex), options);
            argument.Value.StringValue = options[newIndex];
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
