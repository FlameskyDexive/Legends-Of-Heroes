using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ET
{
    public sealed class BehaviorTreeSearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        private readonly struct SearchNodeEntry
        {
            public SearchNodeEntry(BehaviorTreeNodeKind nodeKind, string groupName)
            {
                this.NodeKind = nodeKind;
                this.GroupName = groupName;
            }

            public BehaviorTreeNodeKind NodeKind { get; }

            public string GroupName { get; }
        }

        private static readonly SearchNodeEntry[] SearchEntries =
        {
            new(BehaviorTreeNodeKind.Sequence, "Composites"),
            new(BehaviorTreeNodeKind.Selector, "Composites"),
            new(BehaviorTreeNodeKind.Parallel, "Composites"),
            new(BehaviorTreeNodeKind.Inverter, "Decorators"),
            new(BehaviorTreeNodeKind.Succeeder, "Decorators"),
            new(BehaviorTreeNodeKind.Failer, "Decorators"),
            new(BehaviorTreeNodeKind.Repeater, "Decorators"),
            new(BehaviorTreeNodeKind.BlackboardCondition, "Decorators"),
            new(BehaviorTreeNodeKind.Service, "Decorators"),
            new(BehaviorTreeNodeKind.SubTree, "Decorators"),
            new(BehaviorTreeNodeKind.Action, "Actions"),
            new(BehaviorTreeNodeKind.Condition, "Actions"),
            new(BehaviorTreeNodeKind.Wait, "Actions"),
        };

        private Texture2D indentationIcon;
        private BehaviorTreeEditorWindow window;
        private BehaviorTreeGraphView graphView;

        public void Initialize(BehaviorTreeEditorWindow window, BehaviorTreeGraphView graphView)
        {
            this.window = window;
            this.graphView = graphView;

            if (this.indentationIcon == null)
            {
                this.indentationIcon = new Texture2D(1, 1);
                this.indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
                this.indentationIcon.Apply();
                this.indentationIcon.hideFlags = HideFlags.HideAndDontSave;
            }
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> entries = new()
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node"), 0),
            };

            string currentGroup = string.Empty;
            foreach (SearchNodeEntry entry in SearchEntries)
            {
                if (!string.Equals(currentGroup, entry.GroupName, StringComparison.Ordinal))
                {
                    currentGroup = entry.GroupName;
                    entries.Add(new SearchTreeGroupEntry(new GUIContent(currentGroup), 1));
                }

                entries.Add(new SearchTreeEntry(new GUIContent(BehaviorTreeEditorUtility.GetDefaultTitle(entry.NodeKind), this.indentationIcon))
                {
                    level = 2,
                    userData = entry.NodeKind,
                });
            }

            return entries;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            if (searchTreeEntry.userData is not BehaviorTreeNodeKind nodeKind)
            {
                return false;
            }

            Vector2 windowMousePosition = context.screenMousePosition - this.window.position.position;
            Vector2 graphMousePosition = this.graphView.contentViewContainer.WorldToLocal(windowMousePosition);
            this.graphView.CreateNode(nodeKind, graphMousePosition);
            return true;
        }

        private void OnDisable()
        {
            if (this.indentationIcon != null)
            {
                DestroyImmediate(this.indentationIcon);
                this.indentationIcon = null;
            }
        }
    }
}
