using System.Collections.Generic;

namespace ET
{
    [BTNodeDescriptor]
    public sealed class BTLogNodeDescriptor : ABTNodeDescriptor
    {
        public override string TypeId => BTBuiltinNodeTypes.Log;

        public override BTNodeKind NodeKind => BTNodeKind.Action;

        public override string MenuPath => "Behaviors/Common/Log";

        public override string HandlerName => "Log";

        public override string Description => "Writes a log message and helps verify behavior tree execution order.";

        public override IReadOnlyList<BTNodeParameterDefinition> Parameters => new List<BTNodeParameterDefinition>
        {
            new()
            {
                Name = "message",
                DisplayName = "Message",
                ValueType = BTValueType.String,
                DefaultValue = new BTSerializedValue
                {
                    ValueType = BTValueType.String,
                    StringValue = "BehaviorTree Log",
                },
                EditorHint = BTNodeParameterEditorHint.MultilineText,
            },
        };
    }

    [BTNodeDescriptor]
    public sealed class BTSetBlackboardNodeDescriptor : ABTNodeDescriptor
    {
        public override string TypeId => BTBuiltinNodeTypes.SetBlackboard;

        public override BTNodeKind NodeKind => BTNodeKind.Action;

        public override string MenuPath => "Behaviors/Blackboard/Set Value";

        public override string HandlerName => "SetBlackboard";

        public override string Description => "Writes a value to the blackboard, or removes the key when Remove is enabled.";

        public override IReadOnlyList<BTNodeParameterDefinition> Parameters => new List<BTNodeParameterDefinition>
        {
            new()
            {
                Name = "key",
                DisplayName = "Blackboard Key",
                ValueType = BTValueType.String,
                DefaultValue = new BTSerializedValue
                {
                    ValueType = BTValueType.String,
                    StringValue = string.Empty,
                },
                EditorHint = BTNodeParameterEditorHint.BlackboardKey,
            },
            new()
            {
                Name = "remove",
                DisplayName = "Remove",
                ValueType = BTValueType.Boolean,
                DefaultValue = new BTSerializedValue
                {
                    ValueType = BTValueType.Boolean,
                    BoolValue = false,
                },
            },
            new()
            {
                Name = "value",
                DisplayName = "Value",
                ValueType = BTValueType.None,
                DefaultValue = new BTSerializedValue
                {
                    ValueType = BTValueType.String,
                    StringValue = string.Empty,
                },
            },
        };
    }

    [BTNodeDescriptor]
    public sealed class BTBlackboardExistsNodeDescriptor : ABTNodeDescriptor
    {
        public override string TypeId => BTBuiltinNodeTypes.BlackboardExists;

        public override BTNodeKind NodeKind => BTNodeKind.Condition;

        public override string MenuPath => "Conditions/Blackboard/Is Set";

        public override string HandlerName => "BlackboardExists";

        public override string Description => "Checks whether the specified blackboard key currently exists.";

        public override IReadOnlyList<BTNodeParameterDefinition> Parameters => new List<BTNodeParameterDefinition>
        {
            new()
            {
                Name = "key",
                DisplayName = "Blackboard Key",
                ValueType = BTValueType.String,
                DefaultValue = new BTSerializedValue
                {
                    ValueType = BTValueType.String,
                    StringValue = string.Empty,
                },
                EditorHint = BTNodeParameterEditorHint.BlackboardKey,
            },
        };
    }

    [BTNodeDescriptor]
    public sealed class BTBlackboardCompareNodeDescriptor : ABTNodeDescriptor
    {
        public override string TypeId => BTBuiltinNodeTypes.BlackboardCompare;

        public override BTNodeKind NodeKind => BTNodeKind.Condition;

        public override string MenuPath => "Conditions/Blackboard/Compare";

        public override string HandlerName => "BlackboardCompare";

        public override string Description => "Reads a blackboard value and compares it against the configured operand.";

        public override IReadOnlyList<BTNodeParameterDefinition> Parameters => new List<BTNodeParameterDefinition>
        {
            new()
            {
                Name = "key",
                DisplayName = "Blackboard Key",
                ValueType = BTValueType.String,
                DefaultValue = new BTSerializedValue
                {
                    ValueType = BTValueType.String,
                    StringValue = string.Empty,
                },
                EditorHint = BTNodeParameterEditorHint.BlackboardKey,
            },
            new()
            {
                Name = "operator",
                DisplayName = "Operator",
                ValueType = BTValueType.Integer,
                DefaultValue = new BTSerializedValue
                {
                    ValueType = BTValueType.Integer,
                    IntValue = (int)BTCompareOperator.Equal,
                },
                EditorHint = BTNodeParameterEditorHint.CompareOperator,
            },
            new()
            {
                Name = "value",
                DisplayName = "Compare Value",
                ValueType = BTValueType.None,
                DefaultValue = new BTSerializedValue
                {
                    ValueType = BTValueType.String,
                    StringValue = string.Empty,
                },
            },
        };
    }
}
