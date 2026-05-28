using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ET
{
    public sealed class BTreeSearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        private readonly struct SearchNodeEntry
        {
            public SearchNodeEntry(string menuPath, BTreeNodeKind nodeKind, string nodeTypeId = "")
            {
                this.MenuPath = menuPath;
                this.NodeKind = nodeKind;
                this.NodeTypeId = nodeTypeId;
            }

            public string MenuPath { get; }

            public BTreeNodeKind NodeKind { get; }

            public string NodeTypeId { get; }
        }

        private static readonly SearchNodeEntry[] BuiltinSearchEntries =
        {
            new("Composites/Sequence", BTreeNodeKind.Sequence),
            new("Composites/Selector", BTreeNodeKind.Selector),
            new("Composites/Parallel", BTreeNodeKind.Parallel),
            new("Decorators/Inverter", BTreeNodeKind.Inverter),
            new("Decorators/Succeeder", BTreeNodeKind.Succeeder),
            new("Decorators/Failer", BTreeNodeKind.Failer),
            new("Decorators/Repeater", BTreeNodeKind.Repeater),
            new("Decorators/Blackboard Condition", BTreeNodeKind.BlackboardCondition),
            new("Decorators/SubTree", BTreeNodeKind.SubTree),
            new("Behaviors/Common/Wait", BTreeNodeKind.Wait),
            new("Behaviors/Legacy/Custom Action", BTreeNodeKind.Action),
            new("Conditions/Legacy/Custom Condition", BTreeNodeKind.Condition),
            new("Services/Legacy/Custom Service", BTreeNodeKind.Service),
        };

        private Texture2D indentationIcon;
        private BTreeGraphView graphView;

        public void Initialize(BTreeEditorWindow window, BTreeGraphView graphView)
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

            foreach (ABTreeNodeDescriptor descriptor in BTreeEditorUtility.GetAllNodeDescriptors())
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
