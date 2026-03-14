using System;
using System.Collections.Generic;
using System.Linq;

namespace ET
{
    public enum BTNodeParameterEditorHint
    {
        Default = 0,
        BlackboardKey = 1,
        CompareOperator = 2,
        MultilineText = 3,
    }

    [Serializable]
    public sealed class BTNodeParameterDefinition
    {
        public string Name = string.Empty;
        public string DisplayName = string.Empty;
        public BTValueType ValueType = BTValueType.None;
        public BTSerializedValue DefaultValue = new();
        public string Description = string.Empty;
        public BTNodeParameterEditorHint EditorHint;

        public BTNodeParameterDefinition Clone()
        {
            return new BTNodeParameterDefinition
            {
                Name = this.Name,
                DisplayName = this.DisplayName,
                ValueType = this.ValueType,
                DefaultValue = this.DefaultValue?.Clone() ?? new BTSerializedValue(),
                Description = this.Description,
                EditorHint = this.EditorHint,
            };
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class BTNodeDescriptorAttribute : Attribute
    {
    }

    [BTNodeDescriptor]
    public abstract class ABTNodeDescriptor
    {
        public abstract string TypeId { get; }

        public abstract BTNodeKind NodeKind { get; }

        public abstract string MenuPath { get; }

        public virtual string Title => this.MenuPath.Split('/').LastOrDefault() ?? this.TypeId;

        public virtual string HandlerName => string.Empty;

        public virtual string Description => string.Empty;

        public virtual int SortOrder => 0;

        public virtual IReadOnlyList<BTNodeParameterDefinition> Parameters => Array.Empty<BTNodeParameterDefinition>();
    }
}
