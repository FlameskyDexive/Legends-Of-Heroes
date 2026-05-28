using System;
using System.Collections.Generic;
using System.Linq;

namespace ET
{
    public enum BTreeNodeParameterEditorHint
    {
        Default = 0,
        BlackboardKey = 1,
        CompareOperator = 2,
        MultilineText = 3,
    }

    [Serializable]
    public sealed class BTreeNodeParameterDefinition
    {
        public string Name = string.Empty;
        public string DisplayName = string.Empty;
        public BTreeValueType ValueType = BTreeValueType.None;
        public BTreeSerializedValue DefaultValue = new();
        public string Description = string.Empty;
        public BTreeNodeParameterEditorHint EditorHint;

        public BTreeNodeParameterDefinition Clone()
        {
            return new BTreeNodeParameterDefinition
            {
                Name = this.Name,
                DisplayName = this.DisplayName,
                ValueType = this.ValueType,
                DefaultValue = this.DefaultValue?.Clone() ?? new BTreeSerializedValue(),
                Description = this.Description,
                EditorHint = this.EditorHint,
            };
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class BTreeNodeDescriptorAttribute : Attribute
    {
    }

    [BTreeNodeDescriptor]
    public abstract class ABTreeNodeDescriptor
    {
        public abstract string TypeId { get; }

        public abstract BTreeNodeKind NodeKind { get; }

        public abstract string MenuPath { get; }

        public virtual string Title => this.MenuPath.Split('/').LastOrDefault() ?? this.TypeId;

        public virtual string HandlerName => string.Empty;

        public virtual string Description => string.Empty;

        public virtual int SortOrder => 0;

        public virtual IReadOnlyList<BTreeNodeParameterDefinition> Parameters => Array.Empty<BTreeNodeParameterDefinition>();
    }
}
