using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ET
{
    public sealed class BehaviorTreeSearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        private readonly struct SearchNodeEntry
        {
            public SearchNodeEntry(string menuPath, BehaviorTreeNodeKind nodeKind, string nodeTypeId = "")
            {
                this.MenuPath = menuPath;
                this.NodeKind = nodeKind;
                this.NodeTypeId = nodeTypeId;
            }

            public string MenuPath { get; }

            public BehaviorTreeNodeKind NodeKind { get; }

            public string NodeTypeId { get; }
        }

        private static readonly SearchNodeEntry[] BuiltinSearchEntries =
        {
            new("Composites/Sequence", BehaviorTreeNodeKind.Sequence),
            new("Composites/Selector", BehaviorTreeNodeKind.Selector),
            new("Composites/Parallel", BehaviorTreeNodeKind.Parallel),
            new("Decorators/Inverter", BehaviorTreeNodeKind.Inverter),
            new("Decorators/Succeeder", BehaviorTreeNodeKind.Succeeder),
            new("Decorators/Failer", BehaviorTreeNodeKind.Failer),
            new("Decorators/Repeater", BehaviorTreeNodeKind.Repeater),
            new("Decorators/Blackboard Condition", BehaviorTreeNodeKind.BlackboardCondition),
            new("Decorators/SubTree", BehaviorTreeNodeKind.SubTree),
            new("Behaviors/Common/Wait", BehaviorTreeNodeKind.Wait),
            new("Behaviors/Legacy/Custom Action", BehaviorTreeNodeKind.Action),
            new("Conditions/Legacy/Custom Condition", BehaviorTreeNodeKind.Condition),
            new("Services/Legacy/Custom Service", BehaviorTreeNodeKind.Service),
        };

        private Texture2D indentationIcon;
        private BehaviorTreeGraphView graphView;

        public void Initialize(BehaviorTreeEditorWindow window, BehaviorTreeGraphView graphView)
        {
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

            HashSet<string> createdGroups = new(StringComparer.OrdinalIgnoreCase);
            foreach (SearchNodeEntry entry in GetSearchEntries())
            {
                string[] segments = entry.MenuPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                for (int index = 0; index < segments.Length - 1; ++index)
                {
                    string groupPath = string.Join("/", segments.Take(index + 1));
                    if (!createdGroups.Add(groupPath))
                    {
                        continue;
                    }

                    entries.Add(new SearchTreeGroupEntry(new GUIContent(segments[index]), index + 1));
                }

                entries.Add(new SearchTreeEntry(new GUIContent(segments[^1], this.indentationIcon))
                {
                    level = segments.Length,
                    userData = entry,
                });
            }

            return entries;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            if (searchTreeEntry.userData is not SearchNodeEntry nodeEntry)
            {
                return false;
            }

            Vector2 contentPosition = this.graphView.GetPendingNodeCreationContentPosition();
            this.graphView.CreateNodeAtContentPosition(nodeEntry.NodeKind, contentPosition, nodeEntry.NodeTypeId);
            return true;
        }

        private static IEnumerable<SearchNodeEntry> GetSearchEntries()
        {
            foreach (SearchNodeEntry entry in BuiltinSearchEntries)
            {
                yield return entry;
            }

            foreach (ABehaviorTreeNodeDescriptor descriptor in BehaviorTreeEditorUtility.GetAllNodeDescriptors())
            {
                if (descriptor == null || string.IsNullOrWhiteSpace(descriptor.MenuPath))
                {
                    continue;
                }

                yield return new SearchNodeEntry(descriptor.MenuPath, descriptor.NodeKind, descriptor.TypeId);
            }
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
