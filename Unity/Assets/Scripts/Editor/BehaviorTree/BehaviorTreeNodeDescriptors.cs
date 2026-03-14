using System;
using System.Collections.Generic;
using System.Linq;

namespace ET
{
    public enum BehaviorTreeNodeParameterEditorHint
    {
        Default = 0,
        BlackboardKey = 1,
        CompareOperator = 2,
        MultilineText = 3,
    }

    [Serializable]
    public sealed class BehaviorTreeNodeParameterDefinition
    {
        public string Name = string.Empty;
        public string DisplayName = string.Empty;
        public BehaviorTreeValueType ValueType = BehaviorTreeValueType.None;
        public BehaviorTreeSerializedValue DefaultValue = new();
        public string Description = string.Empty;
        public BehaviorTreeNodeParameterEditorHint EditorHint;

        public BehaviorTreeNodeParameterDefinition Clone()
        {
            return new BehaviorTreeNodeParameterDefinition
            {
                Name = this.Name,
                DisplayName = this.DisplayName,
                ValueType = this.ValueType,
                DefaultValue = this.DefaultValue?.Clone() ?? new BehaviorTreeSerializedValue(),
                Description = this.Description,
                EditorHint = this.EditorHint,
            };
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class BehaviorTreeNodeDescriptorAttribute : Attribute
    {
    }

    [BehaviorTreeNodeDescriptor]
    public abstract class ABehaviorTreeNodeDescriptor
    {
        public abstract string TypeId { get; }

        public abstract BehaviorTreeNodeKind NodeKind { get; }

        public abstract string MenuPath { get; }

        public virtual string Title => this.MenuPath.Split('/').LastOrDefault() ?? this.TypeId;

        public virtual string HandlerName => string.Empty;

        public virtual string Description => string.Empty;

        public virtual int SortOrder => 0;

        public virtual IReadOnlyList<BehaviorTreeNodeParameterDefinition> Parameters => Array.Empty<BehaviorTreeNodeParameterDefinition>();
    }
}
