using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ET
{
    public sealed class BTSearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        private readonly struct SearchNodeEntry
        {
            public SearchNodeEntry(string menuPath, BTNodeKind nodeKind, string nodeTypeId = "")
            {
                this.MenuPath = menuPath;
                this.NodeKind = nodeKind;
                this.NodeTypeId = nodeTypeId;
            }

            public string MenuPath { get; }

            public BTNodeKind NodeKind { get; }

            public string NodeTypeId { get; }
        }

        private static readonly SearchNodeEntry[] BuiltinSearchEntries =
        {
            new("Composites/Sequence", BTNodeKind.Sequence),
            new("Composites/Selector", BTNodeKind.Selector),
            new("Composites/Parallel", BTNodeKind.Parallel),
            new("Decorators/Inverter", BTNodeKind.Inverter),
            new("Decorators/Succeeder", BTNodeKind.Succeeder),
            new("Decorators/Failer", BTNodeKind.Failer),
            new("Decorators/Repeater", BTNodeKind.Repeater),
            new("Decorators/Blackboard Condition", BTNodeKind.BlackboardCondition),
            new("Decorators/SubTree", BTNodeKind.SubTree),
            new("Behaviors/Common/Wait", BTNodeKind.Wait),
            new("Behaviors/Legacy/Custom Action", BTNodeKind.Action),
            new("Conditions/Legacy/Custom Condition", BTNodeKind.Condition),
            new("Services/Legacy/Custom Service", BTNodeKind.Service),
        };

        private Texture2D indentationIcon;
        private BTGraphView graphView;

        public void Initialize(BTEditorWindow window, BTGraphView graphView)
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

            foreach (ABTNodeDescriptor descriptor in BTEditorUtility.GetAllNodeDescriptors())
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
